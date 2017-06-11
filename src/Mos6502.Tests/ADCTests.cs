using Xunit;

namespace Mos6502.Tests
{
    public class ADCTests
    {
        [Fact]
        public void ADC_Immediate()
        {
            string code =
@"ADC #01
ADC #02
ADC #03
ADC #F0";
            AssembledProgram program = Assembler.Assemble(code, 0x600);
            Mos6502Cpu cpu = new Mos6502Cpu();
            cpu.LoadProgram(program, 0x600);

            cpu.ProcessInstruction();
            Assert.Equal(0x01, cpu.A);

            cpu.ProcessInstruction();
            Assert.Equal(0x03, cpu.A);

            cpu.ProcessInstruction();
            Assert.Equal(0x06, cpu.A);

            cpu.ProcessInstruction();
            Assert.Equal(0xF6, cpu.A);
        }

        [Fact]
        public void ADC_Immediate_OverflowSetsCarry()
        {
            string code =
@"ADC #FF
ADC #01";
            AssembledProgram program = Assembler.Assemble(code, 0x600);
            Mos6502Cpu cpu = new Mos6502Cpu();
            cpu.LoadProgram(program, 0x600);

            cpu.ProcessInstruction();
            Assert.Equal(0xFF, cpu.A);
            Assert.Equal(false, cpu.CarryFlag);

            cpu.ProcessInstruction();
            Assert.Equal(0x00, cpu.A);
            Assert.Equal(true, cpu.CarryFlag);
        }

        [Fact]
        public void ADC_Absolute()
        {
            var cpu = TestUtil.CpuWithProgram(
@"ADC $0300");
            cpu.Memory.WriteU8(0x300, 0xA);
            cpu.ProcessInstruction();
            Assert.Equal(0xA, cpu.A);
        }

        [Fact]
        public void ADC_Absolute_OverflowSetsCarry()
        {
            var cpu = TestUtil.CpuWithProgram(
@"ADC $0300
ADC $0301");
            cpu.Memory.WriteU16(0x300, 0xFF01);
            cpu.ProcessInstruction();
            Assert.Equal(false, cpu.CarryFlag);
            cpu.ProcessInstruction();
            Assert.Equal(0, cpu.A);
            Assert.Equal(true, cpu.CarryFlag);
        }

        [Fact]
        public void ADC_ZeroPage()
        {
            var cpu = TestUtil.CpuWithProgram("ADC $50");
            cpu.Memory.WriteU8(0x50, 0x10);
            cpu.ProcessInstruction();
            Assert.Equal(0x10, cpu.A);
        }

        [Fact]
        public void ADC_ZeroPageXIndexed()
        {
            var cpu = TestUtil.CpuWithProgram(
@"LDX #05
ADC $50,X");
            cpu.Memory.WriteU8(0x55, 0x10);
            cpu.ProcessInstruction(2);
            Assert.Equal(0x10, cpu.A);
        }

        [Fact]
        public void ADC_AbsoluteXIndexed()
        {
            var cpu = TestUtil.CpuWithProgram(
@"LDX #05
ADC $4000,X");
            cpu.Memory.WriteU8(0x4005, 0x10);
            cpu.ProcessInstruction(2);
            Assert.Equal(0x10, cpu.A);
        }

        [Fact]
        public void ADC_AbsoluteYIndexed()
        {
            var cpu = TestUtil.CpuWithProgram(
@"LDY #05
ADC $4000,Y");
            cpu.Memory.WriteU8(0x4005, 0x10);
            cpu.ProcessInstruction(2);
            Assert.Equal(0x10, cpu.A);
        }

        [Fact]
        public void ADC_XIndexedIndirect()
        {
            var cpu = TestUtil.CpuWithProgram(
@"LDX #05
ADC ($50,X)");
            cpu.Memory.WriteU16(0x55, 0x4000);
            cpu.Memory.WriteU8(0x4000, 0xFF);
            cpu.ProcessInstruction(2);
            Assert.Equal(0xFF, cpu.A);
        }

        [Fact]
        public void ADC_IndirectYIndexed()
        {
            var cpu = TestUtil.CpuWithProgram(
@"LDY #05
ADC ($50),Y");
            cpu.Memory.WriteU16(0x50, 0x4000);
            cpu.Memory.WriteU8(0x4005, 0xFF);
            cpu.ProcessInstruction(2);
            Assert.Equal(0xFF, cpu.A);
        }
    }
}
