using System;
using Xunit;

namespace Mos6502.Tests
{
    public class Mos6502Tests
    {
        [Fact]
        public void SimpleProgram_CheckState()
        {
            string code =
@"LDA #$01
LDX #$02
STA $FF00";
            AssembledProgram program = Assembler.Assemble(code, 0x600);
            Mos6502Cpu cpu = new Mos6502Cpu();
            cpu.LoadProgram(program, 0x600);
            cpu.ProcessInstruction();
            Assert.Equal(0x01, cpu.A);
            cpu.ProcessInstruction();
            Assert.Equal(0x02, cpu.X);
            cpu.ProcessInstruction();
            Assert.Equal(0x01, cpu.Memory.ReadU8(0xFF00));
        }
    }
}
