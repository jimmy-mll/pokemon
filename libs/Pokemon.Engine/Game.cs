using Raylib_CsLo;
using System.Diagnostics;
using System.Reflection;

namespace Pokemon.Engine;

public abstract class Game : IDisposable
{
    private float _lastDt;

    protected virtual void Update(float dt) { }
    protected virtual void Draw() { }

    public void Run()
    {
        Raylib.InitWindow(1280, 720, Assembly.GetExecutingAssembly().FullName ?? "Game");

        var sw = Stopwatch.StartNew();

        while (!Raylib.WindowShouldClose())
        {
            sw.Reset();
            sw.Start();

            Update(_lastDt);

            Raylib.BeginDrawing();

            Draw();

            Raylib.EndDrawing();

            sw.Stop();
            _lastDt = (float)sw.Elapsed.TotalSeconds;
        }

        Raylib.CloseWindow();
    }

    public void Dispose()
    {
        
    }
}