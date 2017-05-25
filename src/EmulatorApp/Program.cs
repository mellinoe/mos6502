using Mos6502;
using System;

namespace EmulatorApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            byte[] bytes = Assembler.Assemble("testprogram.asm");
            Mos6502.Mos6502Cpu cpu = new Mos6502.Mos6502Cpu();
            cpu.LoadProgram(bytes, 0x600, 0x600);
        }
    }
}
