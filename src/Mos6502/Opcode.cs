namespace Mos6502
{
    public enum Opcode : byte
    {
        // ADC
        ADC_Immediate = 0x69,
        ADC_ZeroPage = 0x65,
        ADC_ZeroPageXIndexed = 0x75,
        ADC_Absolute = 0x6D,
        ADC_AbsoluteXIndexed = 0x7D,
        ADC_AbsoluteYIndexed = 0x79,
        ADC_XIndexedIndirect = 0x61,
        ADC_IndirectYIndexed = 0x71,

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
        LDX_ZeroPage = 0xA6,
        LDX_ZeroPageYIndexed = 0xB6,
        LDX_Absolute = 0xAE,
        LDX_AbsoluteYIndexed = 0xBE,

        // LDA
        LDA_Immediate = 0xA9,
        LDA_ZeroPage = 0xA5,
        LDA_ZeroPageXIndexed = 0xB5,
        LDA_Absolute = 0xAD,
        LDA_AbsoluteXIndexed = 0xBD,
        LDA_AbsoluteYIndexed = 0xB9,
        LDA_XIndexedIndirect = 0xA1,
        LDA_IndirectYIndexed = 0xB1,

        // LDY
        LDY_Immediate = 0xA0,
        LDY_ZeroPage = 0xA4,
        LDY_ZeroPageXIndexed = 0xB4,
        LDY_Absolute = 0xAC,
        LDY_AbsoluteXIndexed = 0xBC,

        // INC
        INC_ZeroPage = 0xE6,
        INC_ZeroPageXIndexed = 0xF6,
        INC_Absolute = 0xEE,
        INC_AbsoluteXIndexed = 0xFE,

        INX_Implicit = 0xE8,
        INY_Implicit = 0xC8,
        
        // Transfers
        TXA_Implicit = 0x8A,
        TYA_Implicit = 0x98,
        TXS_Implicit = 0x9A,
        TAY_Implicit = 0xA8,
        TAX_Implicit = 0xAA,
        TSX_Implicit = 0xBA,
    }
}