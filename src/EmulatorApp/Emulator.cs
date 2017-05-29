using Mos6502;
using System;
using OpenTK;
using OpenTK.Graphics;
using Veldrid.Graphics;
using Veldrid.Graphics.OpenGL;
using Veldrid.Platform;
using System.Reflection;
using ImGuiNET;

namespace Mos6502.EmulatorApp
{
    public class Emulator
    {
        private readonly Mos6502Cpu _cpu;

        public Emulator()
        {
            _cpu = new Mos6502Cpu();
        }

        public void UpdateInterface()
        {
            if (ImGui.BeginWindow("MOS 6502 Emulator", WindowFlags.Default))
            {
                ImGui.Text($"A: 0x{_cpu.A.ToString("X2")}");
                ImGui.Text($"X: 0x{_cpu.X.ToString("X2")}");
                ImGui.Text($"Y: 0x{_cpu.Y.ToString("X2")}");
                ImGui.Text($"PC: 0x{_cpu.PC.ToString("X4")}");
                ImGui.Text($"SP: 0x{_cpu.SP.ToString("X2")}");
                if (ImGui.Button("Reset"))
                {
                    _cpu.Reset();
                }
            }
            ImGui.EndWindow();
        }
    }
}
