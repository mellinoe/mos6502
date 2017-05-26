using Mos6502;
using System;
using OpenTK;
using OpenTK.Graphics;
using Veldrid.Graphics;
using Veldrid.Graphics.OpenGL;
using Veldrid.Platform;

namespace Mos6502.EmulatorApp
{
    class Program
    {
        static void Main(string[] args)
        {
            SameThreadWindow window = new SameThreadWindow(1280, 720, Veldrid.Platform.WindowState.Normal);
            window.Title = "MOS 6502 CPU Emulator";
            RenderContext rc = new OpenGLRenderContext(window, false);
            ImGuiRenderer imguiRenderer = new ImGuiRenderer(rc, window.NativeWindow);
            window.Visible = true;
            while (window.Exists)
            {
                var snapshot = window.GetInputSnapshot();
                imguiRenderer.OnInputUpdated(snapshot);
                imguiRenderer.Update(1f / 60f);
                rc.ClearColor = RgbaFloat.Pink;
                rc.Viewport = new Viewport(0, 0,window.Width, window.Height);
                imguiRenderer.Render(rc, "Standard");
                rc.ClearBuffer();
                rc.SwapBuffers();
            }
        }
    }
}
