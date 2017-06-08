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
    }
}
