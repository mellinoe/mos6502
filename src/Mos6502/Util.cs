using System;
using System.Diagnostics;
using System.Text;

namespace Mos6502
{
    internal static class Util
    {
        public static byte GetHighByte(ushort value)
        {
            return (byte)(value >> 8);
        }

        public static byte GetLowByte(ushort value)
        {
            return (byte)(value & 0x00FF);
        }

        public static byte SetBit(byte value, byte position, bool isBitSet)
        {
            Debug.Assert(position <= 7);
            byte bit = isBitSet ? (byte)1 : (byte)0;
            bit = (byte)(bit << position);
            byte mask = (byte)~(1 << position);
            return (byte)((value & mask) | bit);
        }

        public static bool GetBit(byte value, byte position)
        {
            return (value & (1 << position)) != 0;
        }

        public static string GetBitFlagString(byte value)
        {
            StringBuilder sb = new StringBuilder();
            for (byte i = 0; i < 8; i++)
            {
                sb.Append(GetBit(value, i) ? "1" : "0");
            }

            return sb.ToString();
        }

        public static byte GetEncodingLength(AddressMode addressMode)
        {
            switch (addressMode)
            {
                case AddressMode.Accumulator:
                case AddressMode.Implicit:
                    return 0;
                case AddressMode.Immediate:
                case AddressMode.IndirectYIndexed:
                case AddressMode.XIndexedIndirect:
                case AddressMode.Relative:
                case AddressMode.ZeroPage:
                case AddressMode.ZeroPageXIndexed:
                case AddressMode.ZeroPageYIndexed:
                    return 1;
                case AddressMode.Absolute:
                case AddressMode.AbsoluteXIndexed:
                case AddressMode.AbsoluteYIndexed:
                case AddressMode.Indirect:
                    return 2;
                default: throw new InvalidOperationException("Invalid address mode: " + addressMode);
            }
        }
    }
}
