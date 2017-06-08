using Xunit;

namespace Mos6502.Tests
{
    public class LDXTests
    {
        [Fact]
        public void LDX_Immediate()
        {
            var cpu = TestUtil.CpuWithProgram(
@"LDX #01
LDX #02
LDX #03");
            cpu.ProcessInstruction();
            Assert.Equal(1, cpu.X);

            cpu.ProcessInstruction();
            Assert.Equal(2, cpu.X);

            cpu.ProcessInstruction();
            Assert.Equal(3, cpu.X);
        }

        [Fact]
        public void LDX_ZeroPage()
        {
            var cpu = TestUtil.CpuWithProgram(
@"LDA #FF
STA $10
LDX $10");
            cpu.ProcessInstruction(3);
            Assert.Equal(0xFF, cpu.X);
        }

        [Fact]
        public void LDX_ZeroPageYIndexed()
        {
            var cpu = TestUtil.CpuWithProgram(
@"LDA #FF
STA $10
LDY #06
LDX $0A,Y");
            cpu.ProcessInstruction(4);
            Assert.Equal(0xFF, cpu.X);
        }

        [Fact]
        public void LDX_Absolute()
        {
            var cpu = TestUtil.CpuWithProgram(
@"LDA #FF
STA $4000
LDX $4000");
            cpu.ProcessInstruction(3);
            Assert.Equal(0xFF, cpu.X);
        }

        [Fact]
        public void LDX_AbsoluteYIndexed()
        {
            var cpu = TestUtil.CpuWithProgram(
@"LDA #FF
STA $4010
LDY #10
LDX $4000,Y");
            cpu.ProcessInstruction(4);
            Assert.Equal(0xFF, cpu.X);
        }
    }
}
