using Xunit;

namespace Mos6502.Tests
{
    public class STATests
    {
        [Fact]
        public void STA_ZeroPage()
        {
            var cpu = TestUtil.CpuWithProgram(
@"LDA #FF
STA $00");
            cpu.ProcessInstruction();
            cpu.ProcessInstruction();
            Assert.Equal(0xFF, cpu.Memory.ReadU8(0));
        }

        [Fact]
        public void STA_ZeroPageXIndexed()
        {
            var cpu = TestUtil.CpuWithProgram(
@"LDA #FF
LDX #05
STA $05,X");
            cpu.ProcessInstruction();
            cpu.ProcessInstruction();
            cpu.ProcessInstruction();
            Assert.Equal(0, cpu.Memory.ReadU8(0));
            Assert.Equal(0xFF, cpu.Memory.ReadU8(0xA));
        }

        [Fact]
        public void STA_ZeroPageXIndexed_AddressWrap()
        {
            var cpu = TestUtil.CpuWithProgram(
@"LDA #FF
LDX #06
STA $FF,X");
            cpu.ProcessInstruction();
            cpu.ProcessInstruction();
            cpu.ProcessInstruction();
            Assert.Equal(0, cpu.Memory.ReadU8(0));
            Assert.Equal(0xFF, cpu.Memory.ReadU8(0x5));
        }

        [Fact]
        public void STA_AbsoluteXIndexed()
        {
            var cpu = TestUtil.CpuWithProgram(
@"LDA #FF
LDX #05
STA $4000,X");
            cpu.ProcessInstruction();
            cpu.ProcessInstruction();
            cpu.ProcessInstruction();
            Assert.Equal(0xFF, cpu.Memory.ReadU8(0x4005));
        }

        [Fact]
        public void STA_AbsoluteYIndexed()
        {
            var cpu = TestUtil.CpuWithProgram(
@"LDA #FF
LDY #05
STA $4000,Y");
            cpu.ProcessInstruction();
            cpu.ProcessInstruction();
            cpu.ProcessInstruction();
            Assert.Equal(0xFF, cpu.Memory.ReadU8(0x4005));
        }

        [Fact]
        public void STA_XIndexedIndirect()
        {
            var cpu = TestUtil.CpuWithProgram(
@"LDA #FF
LDX #30
STA ($20,X)");
            cpu.Memory.WriteU16(0x50, 0x1234);

            cpu.ProcessInstruction(3);
            Assert.Equal(0xFF, cpu.Memory.ReadU8(0x1234));
        }

        [Fact]
        public void STA_IndirectYIndexed()
        {
            var cpu = TestUtil.CpuWithProgram(
@"LDA #FF
LDY #10
STA ($50),Y");
            cpu.Memory.WriteU16(0x50, 0x1224);
            cpu.ProcessInstruction(3);
            Assert.Equal(0xFF, cpu.Memory.ReadU8(0x1234));
        }
    }
}
