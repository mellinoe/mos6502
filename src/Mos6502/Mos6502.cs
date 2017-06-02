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

        private Memory _memory = new Memory(0x10000);

        public byte A => _a;
        public byte X => _x;
        public byte Y => _y;
        public byte SP => _sp;
        public ushort PC => _pc;
        public byte P => _p;

        public Memory Memory => _memory;

        public Opcode CurrentOpcode => _memory.ReadU8(_pc);

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
            switch (CurrentOpcode)
            {
                case LDA_Immediate:
                {
                    byte operand = _memory.ReadU8(_pc + 1u);
                    _a = operand;
                    _pc = (ushort)(_pc + 2u);
                    break;
                }
                case LDX_Immediate:
                {
                    byte operand = _memory.ReadU8(_pc + 1u);
                    _x = operand;
                    _pc = (ushort)(_pc + 2u);
                    break;
                }
                case STA_Absolute:
                {
                    ushort operand = _memory.ReadU16(_pc + 1u);
                    _memory.WriteU8(operand, _a);
                    _pc = (ushort)(_pc + 3u);
                    break;
                }
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
