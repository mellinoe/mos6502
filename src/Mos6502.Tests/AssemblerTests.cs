using System;
using Xunit;

namespace Mos6502.Tests
{
    public class AssemblerTests
    {
        [Fact]
        public void Basic()
        {
            string code =
@"LDA #$01
LDX #$02
STA $FF00";
            AssembledProgram program = Assembler.Assemble(code, 0);
            Assert.Equal(7, program.Bytes.Length);
            Assert.Equal(0x01, program.Bytes[1]);
            Assert.Equal(0x02, program.Bytes[3]);
            Assert.Equal(0x00, program.Bytes[5]);
            Assert.Equal(0xFF, program.Bytes[6]);
        }
    }
}
