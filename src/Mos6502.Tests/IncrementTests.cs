using Xunit;

namespace Mos6502.Tests
{
    public class IncrementTests
    {
        [Fact]
        public void INX_Implicit()
        {
            var cpu = TestUtil.CpuWithProgram(
@"INX
INX
INX");
            cpu.ProcessInstruction();
            Assert.Equal(1, cpu.X);

            cpu.ProcessInstruction();
            Assert.Equal(2, cpu.X);

            cpu.ProcessInstruction();
            Assert.Equal(3, cpu.X);
        }

        [Fact]
        public void INY_Implicit()
        {
            var cpu = TestUtil.CpuWithProgram(
@"INY
INY
INY");
            cpu.ProcessInstruction();
            Assert.Equal(1, cpu.Y);

            cpu.ProcessInstruction();
            Assert.Equal(2, cpu.Y);

            cpu.ProcessInstruction();
            Assert.Equal(3, cpu.Y);
        }

        [Fact]
        public void INC_Absolute()
        {
            var cpu = TestUtil.CpuWithProgram(
@"INC $4000
INC $4000
INC $4000");

            cpu.Memory.WriteU8(0x4000, 0x05);
            cpu.ProcessInstruction();
            Assert.Equal(6, cpu.Memory.ReadU8(0x4000));

            cpu.ProcessInstruction();
            Assert.Equal(7, cpu.Memory.ReadU8(0x4000));

            cpu.ProcessInstruction();
            Assert.Equal(8, cpu.Memory.ReadU8(0x4000));
        }

        [Fact]
        public void INC_AbsoluteXIndexed()
        {
            var cpu = TestUtil.CpuWithProgram(
@"LDX #01
INC $3FFF,X
INC $3FFF,X
INC $3FFF,X");

            cpu.Memory.WriteU8(0x4000, 0x05);

            cpu.ProcessInstruction(2);
            Assert.Equal(6, cpu.Memory.ReadU8(0x4000));

            cpu.ProcessInstruction();
            Assert.Equal(7, cpu.Memory.ReadU8(0x4000));

            cpu.ProcessInstruction();
            Assert.Equal(8, cpu.Memory.ReadU8(0x4000));
        }

        [Fact]
        public void INC_ZeroPage()
        {
            var cpu = TestUtil.CpuWithProgram(
@"INC $05
INC $05
INC $05");
            cpu.ProcessInstruction();
            Assert.Equal(1, cpu.Memory.ReadU8(0x05));

            cpu.ProcessInstruction();
            Assert.Equal(2, cpu.Memory.ReadU8(0x05));

            cpu.ProcessInstruction();
            Assert.Equal(3, cpu.Memory.ReadU8(0x05));
        }

        [Fact]
        public void INC_ZeroPageXIndexed()
        {
            var cpu = TestUtil.CpuWithProgram(
@"LDX #05
INC $05,X
INC $05,X
INC $05,X");
            cpu.ProcessInstruction();
            cpu.ProcessInstruction();
            Assert.Equal(1, cpu.Memory.ReadU8(0x0A));

            cpu.ProcessInstruction();
            Assert.Equal(2, cpu.Memory.ReadU8(0x0A));

            cpu.ProcessInstruction();
            Assert.Equal(3, cpu.Memory.ReadU8(0x0A));
        }
    }
}
