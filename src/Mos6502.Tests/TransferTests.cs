using Xunit;

namespace Mos6502.Tests
{
    public class TransferTests
    {
        [Fact]
        public void TAX_Implicit()
        {
            var cpu = TestUtil.CpuWithProgram(
@"LDX #10
LDA #FF
TAX");
            cpu.ProcessInstruction(3);
            Assert.Equal(0xFF, cpu.X);
            Assert.Equal(cpu.A, cpu.X);
        }

        [Fact]
        public void TXA_Implicit()
        {
            var cpu = TestUtil.CpuWithProgram(
@"LDA #10
LDX #FF
TXA");
            cpu.ProcessInstruction(3);
            Assert.Equal(0xFF, cpu.A);
            Assert.Equal(cpu.X, cpu.A);
        }

        [Fact]
        public void TAY_Implicit()
        {
            var cpu = TestUtil.CpuWithProgram(
@"LDY #10
LDA #FF
TAY");
            cpu.ProcessInstruction(3);
            Assert.Equal(0xFF, cpu.Y);
            Assert.Equal(cpu.A, cpu.Y);
        }

        [Fact]
        public void TYA_Implicit()
        {
            var cpu = TestUtil.CpuWithProgram(
@"LDA #10
LDY #FF
TYA");
            cpu.ProcessInstruction(3);
            Assert.Equal(0xFF, cpu.A);
            Assert.Equal(cpu.Y, cpu.A);
        }

        [Fact]
        public void TSX_Implicit()
        {
            var cpu = TestUtil.CpuWithProgram(
@"LDX #10
TSX");
            cpu.ProcessInstruction(2);
            Assert.Equal(0xFF, cpu.X);
            Assert.Equal(cpu.SP, cpu.X);
        }

        [Fact]
        public void TXS_Implicit()
        {
            var cpu = TestUtil.CpuWithProgram(
@"LDX #F0
TXS");
            cpu.ProcessInstruction(2);
            Assert.Equal(0xF0, cpu.SP);
            Assert.Equal(cpu.X, cpu.SP);
        }
    }
}
