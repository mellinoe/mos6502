using System;
using System.Diagnostics;

namespace Mos6502
{
    /// <summary>
    /// An emulated MOS Technology 6502 processor.
    /// </summary>
    public class Mos6502Cpu
    {
        /// <summary>
        /// Accumulator register.
        /// </summary>
        private byte _a;
        /// <summary>
        /// X index register.
        /// </summary>
        private byte _x;
        /// <summary>
        /// Y index register.
        /// </summary>
        private byte _y;
        /// <summary>
        /// Stack pointer.
        /// </summary>
        private byte _sp;
        /// <summary>
        /// Program counter.
        /// </summary>
        private ushort _pc;
        /// <summary>
        /// Processor flags.
        /// </summary>
        private byte _p;

        // Processor flags:
        // Bit 0: Carry flag
        // Bit 1: Zero flag
        // Bit 2: Interrupt flag
        // Bit 3: Decimal flag
        // Bit 4: Break flag
        // Bit 5: Unused
        // Bit 6: Overflow flag
        // Bit 7: Negative flag

        private Memory _memory = new Memory(0x10000);

        public byte A => _a;
        public byte X => _x;
        public byte Y => _y;
        public byte SP => _sp;
        public ushort PC => _pc;
        public byte P => _p;

        public bool CarryFlag { get => Util.GetBit(_p, 0); set => _p = Util.SetBit(_p, 0, value); }
        public bool ZeroFlag { get => Util.GetBit(_p, 1); set => _p = Util.SetBit(_p, 1, value); }
        public bool InterruptMask { get => Util.GetBit(_p, 2); set => _p = Util.SetBit(_p, 2, value); }
        public bool DecimalFlag { get => Util.GetBit(_p, 3); set => _p = Util.SetBit(_p, 3, value); }
        public bool BreakFlag { get => Util.GetBit(_p, 4); set => _p = Util.SetBit(_p, 4, value); }
        public bool OverflowFlag { get => Util.GetBit(_p, 6); set => _p = Util.SetBit(_p, 6, value); }
        public bool NegativeFlag { get => Util.GetBit(_p, 7); set => _p = Util.SetBit(_p, 7, value); }

        public Memory Memory => _memory;

        public Opcode CurrentOpcode => (Opcode)_memory.ReadU8(_pc);

        public Mos6502Cpu()
        {
            Reset();
        }

        public void Reset()
        {
            _pc = _memory.ReadU16(ResetVectorAddress);
            _sp = 0xFF;
            _a = 0;
            _x = 0;
            _y = 0;
            _p = InitialProcessorFlags;
        }

        public void LoadProgram(AssembledProgram program, uint address)
        {
            if (_memory.LengthInBytes - address < program.Bytes.Length)
            {
                throw new InvalidOperationException("Not enough memory to load the program.");
            }

            _memory.WriteU16(ResetVectorAddress, (ushort)program.InitialProgramCounter);
            _memory.CopyBytes(address, program.Bytes);

            Reset();
        }

        public void ProcessInstruction(uint count)
        {
            for (uint i = 0; i < count; i++)
            {
                ProcessInstruction();
            }
        }

        public void ProcessInstruction()
        {
            Opcode opcode = CurrentOpcode;
            OpcodeInfo info = OpcodeInfo.GetInfo(opcode);
            switch (opcode)
            {
                case Opcode.ADC_Immediate:
                    {
                        if (DecimalFlag)
                        {
                            throw new NotImplementedException("BCD arithmetic is not implemented.");
                        }
                        else
                        {
                            byte operand = ReadOperandU8();
                            AddWithCarry(operand);
                        }
                        break;
                    }
                case Opcode.ADC_ZeroPage:
                case Opcode.ADC_ZeroPageXIndexed:
                case Opcode.ADC_Absolute:
                case Opcode.ADC_AbsoluteXIndexed:
                case Opcode.ADC_AbsoluteYIndexed:
                case Opcode.ADC_XIndexedIndirect:
                case Opcode.ADC_IndirectYIndexed:
                    {
                        if (DecimalFlag)
                        {
                            throw new NotImplementedException("BCD arithmetic is not implemented.");
                        }
                        else
                        {
                            ushort operand = ReadOperand(info.EncodingLength);
                            ushort address = ComputeAddress(info.AddressMode, operand);
                            byte value = _memory.ReadU8(address);
                            AddWithCarry(value);
                        }
                        break;
                    }
                case Opcode.LDA_Immediate:
                    {
                        _a = ReadOperandU8();
                        break;
                    }
                case Opcode.LDA_ZeroPage:
                case Opcode.LDA_ZeroPageXIndexed:
                case Opcode.LDA_Absolute:
                case Opcode.LDA_AbsoluteXIndexed:
                case Opcode.LDA_AbsoluteYIndexed:
                case Opcode.LDA_XIndexedIndirect:
                case Opcode.LDA_IndirectYIndexed:
                    {
                        ushort operand = ReadOperand(info.EncodingLength);
                        ushort address = ComputeAddress(info.AddressMode, operand);
                        _a = _memory.ReadU8(address);
                        break;
                    }

                case Opcode.LDX_Immediate:
                    {
                        _x = ReadOperandU8();
                        break;
                    }
                case Opcode.LDX_Absolute:
                case Opcode.LDX_AbsoluteYIndexed:
                case Opcode.LDX_ZeroPage:
                case Opcode.LDX_ZeroPageYIndexed:
                    {
                        ushort operand = ReadOperand(info.EncodingLength);
                        ushort address = ComputeAddress(info.AddressMode, operand);
                        _x = _memory.ReadU8(address);
                        break;
                    }
                case Opcode.LDY_Immediate:
                    {
                        _y = ReadOperandU8();
                        break;
                    }
                case Opcode.LDY_Absolute:
                case Opcode.LDY_AbsoluteXIndexed:
                case Opcode.LDY_ZeroPage:
                case Opcode.LDY_ZeroPageXIndexed:
                    {
                        ushort operand = ReadOperand(info.EncodingLength);
                        ushort address = ComputeAddress(info.AddressMode, operand);
                        _y = _memory.ReadU8(address);
                        break;
                    }
                case Opcode.STA_Absolute:
                case Opcode.STA_ZeroPage:
                case Opcode.STA_ZeroPageXIndexed:
                case Opcode.STA_AbsoluteXIndexed:
                case Opcode.STA_AbsoluteYIndexed:
                case Opcode.STA_XIndexedIndirect:
                case Opcode.STA_IndirectYIndexed:
                    {
                        ushort operand = ReadOperand(info.EncodingLength);
                        ushort address = ComputeAddress(info.AddressMode, operand);
                        _memory.WriteU8(address, _a);
                        break;
                    }
                case Opcode.TAX_Implicit:
                    {
                        _x = _a;
                        break;
                    }
                case Opcode.TXA_Implicit:
                    {
                        _a = _x;
                        break;
                    }
                case Opcode.TAY_Implicit:
                    {
                        _y = _a;
                        break;
                    }
                case Opcode.TYA_Implicit:
                    {
                        _a = _y;
                        break;
                    }
                case Opcode.TSX_Implicit:
                    {
                        _x = _sp;
                        break;
                    }
                case Opcode.TXS_Implicit:
                    {
                        _sp = _x;
                        break;
                    }
                case Opcode.INX_Implicit:
                    {
                        _x = (byte)(_x + 1);
                        NegativeFlag = Util.GetBit(_x, 7);
                        ZeroFlag = (_x == 0);
                        break;
                    }
                case Opcode.INY_Implicit:
                    {
                        _y = (byte)(_y + 1);
                        NegativeFlag = Util.GetBit(_y, 7);
                        ZeroFlag = (_y == 0);
                        break;
                    }
                case Opcode.INC_Absolute:
                case Opcode.INC_AbsoluteXIndexed:
                case Opcode.INC_ZeroPage:
                case Opcode.INC_ZeroPageXIndexed:
                    {
                        ushort operand = ReadOperand(info.EncodingLength);
                        ushort address = ComputeAddress(info.AddressMode, operand);
                        byte value = _memory.ReadU8(address);
                        value = (byte)(value + 1);
                        _memory.WriteU8(address, value);
                        NegativeFlag = Util.GetBit(value, 7);
                        ZeroFlag = (value == 0);
                        break;
                    }
                default: throw new NotImplementedException("Opcode is not implemented: " + opcode);
            }

            _pc = (ushort)(_pc + OpcodeInfo.GetInfo(opcode).EncodingLength);
        }

        private void AddWithCarry(byte value)
        {
            int result = _a + value;
            if (result > byte.MaxValue)
            {
                CarryFlag = true;
            }

            _a = (byte)result;
        }

        private byte ReadOperandU8()
        {
            return _memory.ReadU8(_pc + 1u);
        }

        private ushort ReadOperandU16()
        {
            return _memory.ReadU16(_pc + 1u);
        }

        private ushort ReadOperand(byte instructionEncodingLength)
        {
            Debug.Assert(instructionEncodingLength == 2 || instructionEncodingLength == 3);
            int operandBytes = instructionEncodingLength - 1;
            return operandBytes == 1 ? ReadOperandU8() : ReadOperandU16();
        }

        private ushort ComputeAddress(AddressMode mode, ushort operand)
        {
            switch (mode)
            {
                case AddressMode.Absolute:
                case AddressMode.Immediate:
                case AddressMode.ZeroPage:
                    return operand;
                case AddressMode.AbsoluteXIndexed:
                    return (ushort)(operand + _x);
                case AddressMode.ZeroPageXIndexed:
                    return (byte)(operand + _x);
                case AddressMode.AbsoluteYIndexed:
                    return (ushort)(operand + _y);
                case AddressMode.ZeroPageYIndexed:
                    return (byte)(operand + _y);
                case AddressMode.Indirect:
                    return _memory.ReadU16(operand);
                case AddressMode.XIndexedIndirect:
                    return _memory.ReadU16((byte)(operand + _x));
                case AddressMode.IndirectYIndexed:
                    return (ushort)(_memory.ReadU16(operand) + _y);
                case AddressMode.Relative:
                    return (ushort)(_pc + operand + 1); // TODO: This is probably wrong.
                default: throw new InvalidOperationException("An address should not be computed with mode " + mode);
            }
        }

        private const uint ResetVectorAddress = 0xFFFC;
        private const uint StackStartAddress = 0x100;
        private const byte InitialProcessorFlags = 0b0000100;
    }

    /// <summary>
    /// Random access memory
    /// </summary>
    public class Memory
    {
        private byte[] _bytes;

        public Memory(uint byteCount)
        {
            _bytes = new byte[byteCount];
        }

        public uint LengthInBytes => (uint)_bytes.Length;

        public ushort ReadU16(uint address)
        {
            byte b0 = ReadU8(address);
            byte b1 = ReadU8(address + 1);
            return (ushort)((b1 << 8) + b0);
        }

        public void WriteU16(uint address, ushort value)
        {
            byte b0 = Util.GetLowByte(value);
            byte b1 = Util.GetHighByte(value);
            WriteU8(address, b0);
            WriteU8(address + 1, b1);
        }

        public byte ReadU8(uint address)
        {
            return _bytes[address];
        }

        public void WriteU8(uint address, byte value)
        {
            _bytes[address] = value;
        }

        public void CopyBytes(uint address, byte[] bytes)
        {
            Buffer.BlockCopy(bytes, 0, _bytes, (int)address, bytes.Length);
        }

        public byte[] RawBytes => _bytes; // TODO: Don't expose this directly.
    }
}
