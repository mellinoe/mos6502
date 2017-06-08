using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;

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
            // TODO: When all opcodes are implemented, stop doing string concatenation and parsing, and hard-code a table.

            string enumString = opcodeName.ToString() + "_" + addressMode.ToString();
            if (!Enum.TryParse(enumString, out Opcode opcode))
            {
                throw NotImplemented(opcodeName, addressMode);
            }
            else
            {
                return (byte)opcode;
            }
        }

        private static Exception NotImplemented(OpcodeName opcodeName, AddressMode addressMode)
        {
            return new NotImplementedException($"Address mode {addressMode} is not implemented for {opcodeName}.");
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
                return AddressMode.Implicit;
            }

            throw new NotImplementedException("Instruction could not be parsed: " + string.Join(" ", tokens));
        }

        private static void WriteInstruction(List<byte> bytes, OpcodeName opcodeName, AddressMode addressMode, string[] tokens)
        {
            byte opcodeEncoding = GetOpcodeEncoding(opcodeName, addressMode);
            bytes.Add(opcodeEncoding);
            uint numInstructionBytes = 1u + Util.GetEncodingLength(addressMode);

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
                .Replace("(", string.Empty).Replace(")", string.Empty).Replace(",", string.Empty)
                .Replace("X", string.Empty).Replace("Y", string.Empty);
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
