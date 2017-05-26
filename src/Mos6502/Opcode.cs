using System;

namespace Mos6502
{
    public struct Opcode : IEquatable<Opcode>
    {
        public readonly byte Value;
        public Opcode(byte value)
        {
            Value = value;
        }

        public static readonly Opcode STA_Absolute = OpcodeConstants.STA_Absolute;
        public static readonly Opcode LDX_Immediate = OpcodeConstants.LDX_Immediate;
        public static readonly Opcode LDA_Immediate = OpcodeConstants.LDA_Immediate;

        public bool Equals(Opcode other) => Value.Equals(other.Value);
        public override bool Equals(object obj) => obj is Opcode o && Equals(o);
        public override int GetHashCode() => Value.GetHashCode();

        public static implicit operator byte(Opcode opcode) => opcode.Value;
        public static implicit operator Opcode(byte value) => new Opcode(value);
        public static bool operator ==(Opcode left, Opcode right) => left.Equals(right);
        public static bool operator !=(Opcode left, Opcode right) => !left.Equals(right);
    }
}