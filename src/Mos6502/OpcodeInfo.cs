using System;
using System.Collections.Generic;

namespace Mos6502
{
    public struct OpcodeInfo
    {
        public readonly byte Opcode;
        public readonly OpcodeName Name;
        public readonly AddressMode AddressMode;
        public readonly byte EncodingLength;

        public OpcodeInfo(byte opcode, OpcodeName name, AddressMode addressMode)
        {
            Opcode = opcode;
            Name = name;
            AddressMode = addressMode;
            EncodingLength = (byte)(1u + Util.GetEncodingLength(addressMode));
        }

        public static OpcodeInfo GetInfo(Opcode opcode)
        {
            return GetInfo((byte)opcode);
        }

        public static OpcodeInfo GetInfo(byte opcode)
        {
            if (!s_infos.TryGetValue(opcode, out OpcodeInfo ret))
            {
                throw new InvalidOperationException("Unknown opcode: " + opcode);
            }

            return ret;
        }

        private static Dictionary<byte, OpcodeInfo> s_infos = GetOpcodeInfos();

        private static Dictionary<byte, OpcodeInfo> GetOpcodeInfos()
        {
            Dictionary<byte, OpcodeInfo> infos = new Dictionary<byte, OpcodeInfo>();

            Opcode[] opcodes = (Opcode[])Enum.GetValues(typeof(Opcode));
            foreach (var opcode in opcodes)
            {
                string[] parts = opcode.ToString().Split('_');
                if (parts.Length != 2)
                {
                    throw new InvalidOperationException("Badly formatted opcode enum value: " + opcode);
                }

                string nameString = parts[0];
                string addressModeString = parts[1];

                if (!Enum.TryParse(nameString, out OpcodeName name))
                {
                    throw new InvalidOperationException("Badly formatted opcode name: " + nameString);
                }
                if (!Enum.TryParse(addressModeString, out AddressMode addressMode))
                {
                    throw new InvalidOperationException("Badly formatted address mode: " + addressModeString);
                }

                infos.Add((byte)opcode, new OpcodeInfo((byte)opcode, name, addressMode));
            }

            return infos;
        }
    }
}
