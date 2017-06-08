namespace Mos6502
{
    public enum Opcode : byte
    {
        // ADC
        ADC_Immediate = 0x69,
        ADC_Absolute = 0x6D,

        // STA
        STA_ZeroPage = 0x85,
        STA_ZeroPageXIndexed = 0x95,
        STA_Absolute = 0x8D,
        STA_AbsoluteXIndexed = 0x9D,
        STA_AbsoluteYIndexed = 0x99,
        STA_XIndexedIndirect = 0x81,
        STA_IndirectYIndexed = 0x91,

        // LDX
        LDX_Immediate = 0xA2,

        // LDA
        LDA_Immediate = 0xA9,
        
        // LDY
        LDY_Immediate = 0xA0,
    }
}