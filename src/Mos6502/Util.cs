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
    }
}
