namespace Mos6502.Tests
{
    public static class TestUtil
    {
        public static Mos6502Cpu CpuWithProgram(string code, uint initialProgramCounter = 0x600)
        {
            AssembledProgram program = Assembler.Assemble(code, initialProgramCounter);
            Mos6502Cpu cpu = new Mos6502Cpu();
            cpu.LoadProgram(program, initialProgramCounter);
            return cpu;
        }
    }
}
