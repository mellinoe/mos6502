namespace Mos6502
{
    public enum Opcode : byte
    {
        ADC_Absolute = OpcodeConstants.ADC_Absolute,
        ADC_Immediate = OpcodeConstants.ADC_Immediate,
        STA_Absolute = OpcodeConstants.STA_Absolute,
        LDX_Immediate = OpcodeConstants.LDX_Immediate,
        LDA_Immediate = OpcodeConstants.LDA_Immediate,
    }
}