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

        [Fact]
        public void LDA_ZeroPage()
        {
            Mos6502Cpu cpu = TestUtil.CpuWithProgram(
@"LDA $10");
            cpu.Memory.WriteU8(0x10, 0xFF);
            cpu.ProcessInstruction();
            Assert.Equal(0xFF, cpu.A);
        }

        [Fact]
        public void LDA_ZeroPageXIndexed()
        {
            Mos6502Cpu cpu = TestUtil.CpuWithProgram(
@"LDX #0F
LDA $01,X");
            cpu.Memory.WriteU8(0x10, 0xFF);
            cpu.ProcessInstruction(2);
            Assert.Equal(0xFF, cpu.A);
        }

        [Fact]
        public void LDA_Absolute()
        {
            Mos6502Cpu cpu = TestUtil.CpuWithProgram(
@"LDA $4000");
            cpu.Memory.WriteU8(0x4000, 0xFF);
            cpu.ProcessInstruction();
            Assert.Equal(0xFF, cpu.A);
        }

        [Fact]
        public void LDA_AbsoluteXIndexed()
        {
            Mos6502Cpu cpu = TestUtil.CpuWithProgram(
@"LDX #05
LDA $3FFB,X");
            cpu.Memory.WriteU8(0x4000, 0xFF);
            cpu.ProcessInstruction(2);
            Assert.Equal(0xFF, cpu.A);
        }

        [Fact]
        public void LDA_AbsoluteYIndexed()
        {
            Mos6502Cpu cpu = TestUtil.CpuWithProgram(
@"LDY #05
LDA $3FFB,Y");
            cpu.Memory.WriteU8(0x4000, 0xFF);
            cpu.ProcessInstruction(2);
            Assert.Equal(0xFF, cpu.A);
        }

        [Fact]
        public void LDA_XIndexedIndirect()
        {
            Mos6502Cpu cpu = TestUtil.CpuWithProgram(
@"LDX #05
LDA ($20,X)");
            cpu.Memory.WriteU16(0x25, 0x4000);
            cpu.Memory.WriteU8(0x4000, 0xFF);
            cpu.ProcessInstruction(2);
            Assert.Equal(0xFF, cpu.A);
        }

        [Fact]
        public void LDA_IndirectYIndexed()
        {
            Mos6502Cpu cpu = TestUtil.CpuWithProgram(
@"LDY #05
LDA ($20),Y");
            cpu.Memory.WriteU16(0x20, 0x4000);
            cpu.Memory.WriteU8(0x4005, 0xFF);
            cpu.ProcessInstruction(2);
            Assert.Equal(0xFF, cpu.A);
        }
    }
}
