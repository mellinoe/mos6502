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
    class Program
    {
        public static RenderContext RenderContext { get; private set; }
        public static ImGuiRenderer ImGuiRenderer { get; private set; }
        public static OpenTKWindow Window { get; private set; }
        public static Emulator Emulator { get; private set; }
        static void Main(string[] args)
        {
            Emulator = new Emulator();
            Window = new SameThreadWindow(1280, 720, Veldrid.Platform.WindowState.Normal);
            Window.Title = "MOS 6502 CPU Emulator";
            RenderContext = new OpenGLRenderContext(Window, false);
            RenderContext.ResourceFactory.AddShaderLoader(new EmbeddedResourceShaderLoader(typeof(Program).GetTypeInfo().Assembly));
            ImGuiRenderer = new ImGuiRenderer(RenderContext, Window.NativeWindow);
            Window.Visible = true;
            DateTime previousFrameTime = DateTime.UtcNow;
            while (Window.Exists)
            {
                DateTime now = DateTime.UtcNow;
                TimeSpan elapsed = now - previousFrameTime;
                float deltaMS = (float)elapsed.TotalSeconds;
                previousFrameTime = now;
                var snapshot = Window.GetInputSnapshot();
                Update(snapshot, deltaMS);
                Draw();
            }
        }

        private static void Update(InputSnapshot snapshot, float deltaSeconds)
        {
            ImGuiRenderer.Update(1f / 60f);
            ImGuiRenderer.OnInputUpdated(snapshot);

            Emulator.UpdateInterface();
        }

        private static void Draw()
        {
            RenderContext.ClearColor = RgbaFloat.Pink;
            RenderContext.Viewport = new Viewport(0, 0, Window.Width, Window.Height);
            RenderContext.ClearBuffer();
            ImGuiRenderer.Render(RenderContext, "Standard");
            RenderContext.SwapBuffers();
        }
    }
}
