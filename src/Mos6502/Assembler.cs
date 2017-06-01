using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;

using static Mos6502.OpcodeConstants;

namespace Mos6502
{
    public static class Assembler
    {
        private static readonly char[] s_lineFeed = { '\n' };

        public static AssembledProgram AssembleFromFile(string path) => AssembleFromFile(path, 0);
        public static AssembledProgram AssembleFromFile(string path, uint initialProgramCounter)
        {
            string[] lines = File.ReadAllLines(path);
            return Assemble(lines, initialProgramCounter);
        }

        public static AssembledProgram Assemble(string code) => Assemble(code, 0);
        public static AssembledProgram Assemble(string code, uint initialProgramCounter)
        {
            string normalized = code.Replace("\r\n", "\n");
            return Assemble(normalized.Split(s_lineFeed), initialProgramCounter);
        }

        public static AssembledProgram Assemble(string[] lines) => Assemble(lines, 0);
        public static AssembledProgram Assemble(string[] lines, uint initialProgramCounter)
        {
            List<byte> bytes = new List<byte>();
            foreach (string line in lines)
            {
                string[] tokens = line.Split(' ');
                if (tokens.Length > 0)
                {
                    string opcodeString = tokens[0];
                    OpcodeName opcode = (OpcodeName)Enum.Parse(typeof(OpcodeName), opcodeString, true);
                    AddressMode addressMode = GetAddressMode(opcode, tokens);
                    WriteInstruction(bytes, opcode, addressMode, tokens);
                }
            }

            return new AssembledProgram(initialProgramCounter, bytes.ToArray());
        }

        public static byte GetOpcodeEncoding(OpcodeName opcodeName, AddressMode addressMode)
        {
            switch (opcodeName)
            {
                case OpcodeName.LDA:
                    switch (addressMode)
                    {
                        case AddressMode.Immediate:
                            return 0xA9;
                        default: throw new NotImplementedException($"Address mode {addressMode} is not implemented for {opcodeName}.");
                    }
                case OpcodeName.LDX:
                    switch (addressMode)
                    {
                        case AddressMode.Immediate:
                            return 0xA2;
                        default: throw new NotImplementedException($"Address mode {addressMode} is not implemented for {opcodeName}.");
                    }
                case OpcodeName.STA:
                    switch (addressMode)
                    {
                        case AddressMode.Absolute:
                            return 0x8D;
                        default: throw new NotImplementedException($"Address mode {addressMode} is not implemented for {opcodeName}.");
                    }
                default: throw new NotImplementedException($"{opcodeName} is not implemented.");
            }
        }

        public static uint GetEncodingLengthInBytes(byte opcode)
        {
            if (!s_opcodeOperandLengths.TryGetValue(opcode, out uint ret))
            {
                throw new InvalidOperationException($"No such opcode: {opcode}");
            }

            return ret;
        }

        private static AddressMode GetAddressMode(OpcodeName opcode, string[] tokens)
        {
            if (tokens.Length >= 2)
            {
                string operand = tokens[1];
                if (operand.StartsWith("#"))
                {
                    return AddressMode.Immediate;
                }
                else if (operand == "A")
                {
                    return AddressMode.Accumulator;
                }
                else if (operand.StartsWith("("))
                {
                    if (operand.Contains(",X)"))
                    {
                        return AddressMode.XIndexedIndirect;
                    }
                    else if (operand.Contains("),Y"))
                    {
                        return AddressMode.IndirectYIndexed;
                    }
                    else
                    {
                        return AddressMode.Indirect;
                    }
                }
                else
                {
                    if (operand.Length == 3) // zpg or rel, depending on opcode
                    {
                        if (opcode == OpcodeName.BPL || opcode == OpcodeName.BMI || opcode == OpcodeName.BVC
                            || opcode == OpcodeName.BVS || opcode == OpcodeName.BCC || opcode == OpcodeName.BCS
                            || opcode == OpcodeName.BNE || opcode == OpcodeName.BEQ)
                        {
                            return AddressMode.Relative;
                        }
                        else
                        {
                            return AddressMode.ZeroPage;
                        }
                    }
                    else if (operand.Length == 5)
                    {
                        if (operand.EndsWith(",X"))
                        {
                            return AddressMode.ZeroPageXIndexed;
                        }
                        else if (operand.EndsWith(",Y"))
                        {
                            return AddressMode.ZeroPageYIndexed;
                        }
                        else
                        {
                            return AddressMode.Absolute;
                        }
                    }
                    else if (operand.Length == 7)
                    {
                        if (operand.EndsWith(",X"))
                        {
                            return AddressMode.AbsoluteXIndexed;
                        }
                        else
                        {
                            return AddressMode.AbsoluteYIndexed;
                        }
                    }
                }
            }
            else
            {
                return AddressMode.Implied;
            }

            throw new NotImplementedException("Instruction could not be parsed: " + string.Join(" ", tokens));
        }

        private static void WriteInstruction(List<byte> bytes, OpcodeName opcodeName, AddressMode addressMode, string[] tokens)
        {
            byte opcodeEncoding = GetOpcodeEncoding(opcodeName, addressMode);
            bytes.Add(opcodeEncoding);
            uint numInstructionBytes = GetEncodingLengthInBytes(opcodeEncoding);

            if (numInstructionBytes == 2)
            {
                // Single operand.
                if (!GetOperandU8(tokens[1], out byte operand))
                {
                    throw new MalformedProgramException($"Not a valid 1-byte operand: {tokens[1]}.");
                }

                bytes.Add(operand);
            }
            else if (numInstructionBytes == 3)
            {
                if (!GetOperandU16(tokens[1], out ushort operand))
                {
                    throw new MalformedProgramException($"Not a valid 2-byte operand: {tokens[1]}");
                }

                bytes.Add(Util.GetLowByte(operand));
                bytes.Add(Util.GetHighByte(operand));
            }
        }

        private static bool GetOperandU8(string operand, out byte value)
        {
            // TODO: Make sure the operand is only one byte, and fail if it isn't.
            return byte.TryParse(NormalizeOperandString(operand), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out value);
        }

        private static bool GetOperandU16(string operand, out ushort value)
        {
            // TODO: Check if the operand is actually long enough to be two bytes, and fail if it isn't.
            return ushort.TryParse(NormalizeOperandString(operand), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out value);
        }

        private static string NormalizeOperandString(string operand)
        {
            return operand.Replace("#", string.Empty).Replace("$", string.Empty)
                .Replace("(", string.Empty).Replace(")", string.Empty).Replace(",", string.Empty);
        }

        private static byte GetImmediateOperandByte(string operand)
        {
            Debug.Assert(operand.StartsWith("#$"));
            byte value = byte.Parse(operand.Substring(2), System.Globalization.NumberStyles.HexNumber);
            return value;
        }

        private static ushort GetAbsoluteOperand(string operand)
        {
            Debug.Assert(operand.StartsWith("$") && operand.Length == 5);
            ushort value = ushort.Parse(operand.Substring(1), System.Globalization.NumberStyles.HexNumber);
            return value;
        }

        private static readonly Dictionary<byte, uint> s_opcodeOperandLengths = new Dictionary<byte, uint>()
        {
            // Includes the 1 byte instruction opcode.
            { STA_Absolute, 3 }, // STA, Absolute
            { LDX_Immediate, 2 }, // LDX, Immediate
            { LDA_Immediate, 2 }, // LDA, Immediate
        };
    }

    public enum OpcodeName
    {
        ADC, // add with carry
        AND, // and (with accumulator)
        ASL, // arithmetic shift left
        BCC, // branch on carry clear
        BCS, // branch on carry set
        BEQ, // branch on equal (zero set)
        BIT, // bit test
        BMI, // branch on minus (negative set)
        BNE, // branch on not equal (zero clear)
        BPL, // branch on plus (negative clear)
        BRK, // interrupt
        BVC, // branch on overflow clear
        BVS, // branch on overflow set
        CLC, // clear carry
        CLD, // clear decimal
        CLI, // clear interrupt disable
        CLV, // clear overflow
        CMP, // compare (with accumulator)
        CPX, // compare with X
        CPY, // compare with Y
        DEC, // decrement
        DEX, // decrement X
        DEY, // decrement Y
        EOR, // exclusive or (with accumulator)
        INC, // increment
        INX, // increment X
        INY, // increment Y
        JMP, // jump
        JSR, // jump subroutine
        LDA, // load accumulator
        LDX, // load X
        LDY, // load Y
        LSR, // logical shift right
        NOP, // no operation
        ORA, // or with accumulator
        PHA, // push accumulator
        PHP, // push processor status (SR)
        PLA, // pull accumulator
        PLP, // pull processor status (SR)
        ROL, // rotate left
        ROR, // rotate right
        RTI, // return from interrupt
        RTS, // return from subroutine
        SBC, // subtract with carry
        SEC, // set carry
        SED, // set decimal
        SEI, // set interrupt disable
        STA, // store accumulator
        STX, // store X
        STY, // store Y
        TAX, // transfer accumulator to X
        TAY, // transfer accumulator to Y
        TSX, // transfer stack pointer to X
        TXA, // transfer X to accumulator
        TXS, // transfer X to stack pointer
        TYA, // transfer Y to accumulator
    }

    public enum AddressMode
    {
        // Name             //     Form          Dissassembly        Description
        Accumulator,        //     A             OPC A               operand is AC
        Absolute,           //     abs           OPC $HHLL           operand is address $HHLL
        AbsoluteXIndexed,   //     abs, X        OPC $HHLL, X        operand is address incremented by X with carry
        AbsoluteYIndexed,   //     abs, Y        OPC $HHLL, Y        operand is address incremented by Y with carry
        Immediate,          //     #             OPC #$BB            operand is byte (BB)
        Implied,            //     impl          OPC                 operand implied
        Indirect,           //     ind           OPC ($HHLL)         operand is effective address; effective address is value of address
        XIndexedIndirect,   //     X, ind        OPC ($BB, X)        operand is effective zeropage address; effective address is byte (BB) incremented by X without carry
        IndirectYIndexed,   //     ind, Y        OPC ($LL), Y        operand is effective address incremented by Y with carry; effective address is word at zeropage address
        Relative,           //     rel           OPC $BB             branch target is PC + offset (BB), bit 7 signifies negative offset
        ZeroPage,           //     zpg           OPC $LL             operand is of address; address hibyte = zero($00xx)
        ZeroPageXIndexed,   //     zpg, X        OPC $LL, X          operand is address incremented by X; address hibyte = zero($00xx); no page transition
        ZeroPageYIndexed,   //     zpg, Y        OPC $LL, Y          operand is address incremented by Y; address hibyte = zero($00xx); no page transition
    }

    public class AssembledProgram
    {
        public uint InitialProgramCounter { get; }
        public byte[] Bytes { get; }

        public AssembledProgram(uint initialProgramCounter, byte[] programBytes)
        {
            InitialProgramCounter = initialProgramCounter;
            Bytes = programBytes;
        }
    }
}
