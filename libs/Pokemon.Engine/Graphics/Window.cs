using Pokemon.Engine.Primitives;
using Raylib_CsLo;
using System.Numerics;

namespace Pokemon.Engine.Graphics;

public class Window : IDisposable
{
    public int Width => _size.X;

    public int Height => _size.Y;

    public Vector2I Position
    {
        get => _position;
        set => SetPosition(value.X, value.Y);
    }

    public float Opacity
    {
        get => _opacity;
        set => SetOpacity(value);
    }

    public bool IsFocused => Raylib.IsWindowFocused();

    public bool IsFullscreen => Raylib.IsWindowFullscreen();

    public bool IsHidden => Raylib.IsWindowHidden();

    public bool IsMaximized => Raylib.IsWindowMaximized();

    public bool IsMinimized => Raylib.IsWindowMinimized();


    public string Title
    {
        get => _title;
        set
        {
            _title = value;
            Raylib.SetWindowTitle(value);
        }
    }

    private float _opacity;
    private string _title;
    private Vector2I _position;
    private Vector2I _size;
    private bool _isDisposed;

    public Window(string title, Vector2I? size = null)
    {
        _title = title;

        size ??= new Vector2I(1280, 720);
        SetSize(size.Value.X, size.Value.Y);

        Raylib.InitWindow(Width, Height, _title);
    }

    ~Window()
    {
        Dispose(disposing: false);
    }

    public void Maximize()
    {
        if (IsMaximized)
            return;

        Raylib.MaximizeWindow();
    }

    public void Minimize()
    {
        if (IsMinimized)
            return;

        Raylib.MinimizeWindow();
    }

    public void ToggleFullscreen()
        => Raylib.ToggleFullscreen();

    public void SetPosition(int x, int y)
    {
        _position = new Vector2I(x, y);
        Raylib.SetWindowPosition(x, y);
    }

    public void SetSize(int width, int height)
    {
        _size = new Vector2I(width, height);
        Raylib.SetWindowSize(width, height);
    }

    public void SetOpacity(float opacity)
    {
        _opacity = Math.Clamp(opacity, 0.0f, 1.0f);
        Raylib.SetWindowOpacity(opacity);
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_isDisposed)
        {
            Raylib.CloseWindow();
            _isDisposed = true;
        }
    }
}
