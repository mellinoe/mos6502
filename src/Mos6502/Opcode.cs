namespace Mos6502
{
    public enum Opcode : byte
    {
        ADC_Immediate = 0x69,
        ADC_Absolute = 0x6D,
        STA_Absolute = 0x9D,
        LDX_Immediate = 0xA2,
        LDA_Immediate = 0xA9,
    }
}