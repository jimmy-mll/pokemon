using Pokemon.Engine.Graphics;
using Raylib_CsLo;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Pokemon.Engine;

public abstract class Game : IDisposable
{
    protected int FPS => Raylib.GetFPS();
    protected bool ShowFPS { get; set; }

    protected int TargetFPS
    {
        get => _targetFPS;
        set
        {
            _targetFPS = value;
            Raylib.SetTargetFPS(value);
        }
    }

    public Window Window => _window;

    private float _lastDt;
    private int _targetFPS;
    private bool _isRunning;
    private Window _window = null!;

    protected virtual void Initialize() { }
    protected virtual void Update(float dt) { }
    protected virtual void Draw() { }

    public void Run()
    {
        if (_isRunning)
            throw new Exception("Couldn't run the game twice.");

        _window = new Window(Assembly.GetExecutingAssembly().FullName ?? "Game");

        Initialize();

        var sw = Stopwatch.StartNew();

        while (!Raylib.WindowShouldClose())
        {
            sw.Reset();
            sw.Start();

            Update(_lastDt);

            Raylib.BeginDrawing();

            Draw();

            if (ShowFPS) Raylib.DrawFPS(20, 20);

            Raylib.EndDrawing();

            sw.Stop();
            _lastDt = (float)sw.Elapsed.TotalSeconds;
        }

        Raylib.CloseWindow();
    }

    public void Dispose()
    {
        Window.Dispose();
    }
}