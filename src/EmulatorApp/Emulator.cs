using System;
using System.Numerics;
using ImGuiNET;

namespace Mos6502.EmulatorApp
{
    public class Emulator
    {
        private readonly Mos6502Cpu _cpu;
        private TextInputBuffer _assemblyTextInput = new TextInputBuffer(2048);
        private string _statusText = string.Empty;
        private MemoryEditor _memoryEditor;

        public Emulator()
        {
            _cpu = new Mos6502Cpu();
            _memoryEditor = new MemoryEditor();
            _memoryEditor.AllowEdits = false;
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

                ImGui.InputTextMultiline("Code", _assemblyTextInput.Buffer, _assemblyTextInput.Length, new Vector2(400, 300), InputTextFlags.Default, null);
                if (ImGui.Button("Load Program"))
                {
                    try
                    {
                        string text = _assemblyTextInput.ToString();
                        var program = Assembler.Assemble(text, 0x100);
                        _cpu.LoadProgram(program, 0x100);
                        _cpu.Reset();
                        _statusText = "Program assembled successfully. Total bytes: " + program.Bytes.Length;
                    }
                    catch (Exception e)
                    {
                        _statusText = "ERROR: " + e;
                    }
                }
                if (ImGui.Button("Cycle"))
                {
                    _cpu.ProcessInstruction();
                }

                ImGui.Text("STATUS: " + _statusText);
            }
            ImGui.EndWindow();

            _memoryEditor.Draw("RAM", _cpu.Memory.RawBytes, (int)_cpu.Memory.LengthInBytes);
        }
    }
}
