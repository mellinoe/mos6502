using System;

using static Mos6502.OpcodeConstants;

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

        public bool DecimalFlag { get => Util.GetBit(_p, 3); set => _p = Util.SetBit(_p, 3, value); }

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

        public void ProcessInstruction()
        {
            Opcode opcode = CurrentOpcode;
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
                case Opcode.ADC_Absolute:
                    {
                        if (DecimalFlag)
                        {
                            throw new NotImplementedException("BCD arithmetic is not implemented.");
                        }
                        else
                        {
                            ushort operand = ReadOperandU16();
                            byte value = _memory.ReadU8(operand);
                            AddWithCarry(value);
                        }
                        break;
                    }
                case Opcode.LDA_Immediate:
                    {
                        byte operand = ReadOperandU8();
                        _a = operand;
                        break;
                    }
                case Opcode.LDX_Immediate:
                    {
                        byte operand = ReadOperandU8();
                        _x = operand;
                        break;
                    }
                case Opcode.STA_Absolute:
                    {
                        ushort operand = ReadOperandU16();
                        _memory.WriteU8(operand, _a);
                        break;
                    }
            }

            _pc = (ushort)(_pc + Assembler.GetEncodingLengthInBytes(opcode));
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
