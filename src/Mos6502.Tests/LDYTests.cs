using Xunit;

namespace Mos6502.Tests
{
    public class LDYTests
    {
        [Fact]
        public void LDY_Immediate()
        {
            var cpu = TestUtil.CpuWithProgram("LDY #FF");
            cpu.ProcessInstruction();
            Assert.Equal(0xFF, cpu.Y);
        }

        [Fact]
        public void LDY_ZeroPage()
        {
            var cpu = TestUtil.CpuWithProgram("LDY $50");
            cpu.Memory.WriteU8(0x50, 0xFF);
            cpu.ProcessInstruction();
            Assert.Equal(0xFF, cpu.Y);
        }

        [Fact]
        public void LDY_ZeroPageXIndexed()
        {
            var cpu = TestUtil.CpuWithProgram(
@"LDX #5
LDY $50,X");
            cpu.Memory.WriteU8(0x55, 0xFF);
            cpu.ProcessInstruction(2);
            Assert.Equal(0xFF, cpu.Y);
        }

        [Fact]
        public void LDY_Absolute()
        {
            var cpu = TestUtil.CpuWithProgram("LDY $4000");
            cpu.Memory.WriteU8(0x4000, 0xFF);
            cpu.ProcessInstruction();
            Assert.Equal(0xFF, cpu.Y);
        }

        [Fact]
        public void LDY_AbsoluteXIndexed()
        {
            var cpu = TestUtil.CpuWithProgram(
@"LDX #05
LDY $4000,X");
            cpu.Memory.WriteU8(0x4005, 0xFF);
            cpu.ProcessInstruction(2);
            Assert.Equal(0xFF, cpu.Y);
        }
    }
}
