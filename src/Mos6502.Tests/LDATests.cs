using Xunit;

namespace Mos6502.Tests
{
    public class LDATests
    {
        [Fact]
        public void LDA_Immediate()
        {
            Mos6502Cpu cpu = TestUtil.CpuWithProgram(
@"LDA #01
LDA #02
LDA #03
LDA #FF
LDA #00");
            cpu.ProcessInstruction();
            Assert.Equal(1, cpu.A);

            cpu.ProcessInstruction();
            Assert.Equal(2, cpu.A);

            cpu.ProcessInstruction();
            Assert.Equal(3, cpu.A);

            cpu.ProcessInstruction();
            Assert.Equal(0xFF, cpu.A);

            cpu.ProcessInstruction();
            Assert.Equal(0, cpu.A);
        }
    }
}
