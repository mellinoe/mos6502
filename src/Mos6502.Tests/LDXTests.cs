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
@"LDX $10");
            cpu.Memory.WriteU8(0x10, 0xFF);

            cpu.ProcessInstruction();
            Assert.Equal(0xFF, cpu.X);
        }

        [Fact]
        public void LDX_ZeroPageYIndexed()
        {
            var cpu = TestUtil.CpuWithProgram(
@"LDY #06
LDX $0A,Y");
            cpu.Memory.WriteU8(0x10, 0xFF);

            cpu.ProcessInstruction(2);
            Assert.Equal(0xFF, cpu.X);
        }

        [Fact]
        public void LDX_Absolute()
        {
            var cpu = TestUtil.CpuWithProgram(
@"LDX $4000");
            cpu.Memory.WriteU8(0x4000, 0xFF);

            cpu.ProcessInstruction();
            Assert.Equal(0xFF, cpu.X);
        }

        [Fact]
        public void LDX_AbsoluteYIndexed()
        {
            var cpu = TestUtil.CpuWithProgram(
@"LDY #10
LDX $4000,Y");
            cpu.Memory.WriteU8(0x4010, 0xFF);

            cpu.ProcessInstruction(2);
            Assert.Equal(0xFF, cpu.X);
        }
    }
}
