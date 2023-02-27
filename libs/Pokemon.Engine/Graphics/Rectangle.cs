using System.Numerics;
using Raylib_CsLo;

namespace Pokemon.Engine.Graphics;

public sealed class Rectangle
{
    public static Rectangle Zero => 
        new(Vector2.Zero, Vector2.Zero);
    
    public static Rectangle Max =>
        new(new Vector2(float.MinValue), new Vector2(float.MaxValue));

    private Raylib_CsLo.Rectangle _base;
    private Vector2 _position;
    private Vector2 _size;
    
    public Vector2 Position
    {
        get => _position;
        set
        {
            _position = value;
            _base.x = value.X;
            _base.y = value.Y;
        }
    }
    
    public Vector2 Size
    {
        get => _size;
        set
        {
            _size = value;
            _base.width = value.X;
            _base.height = value.Y;
        }
    }
    
    public Rectangle(Vector2 position, Vector2 size)
    {
        _base = new Raylib_CsLo.Rectangle(position.X, position.Y, size.X, size.Y);
        _position = position;
        _size = size;
    }
    
    public void Deconstruct(out Vector2 position, out Vector2 size)
    {
        position = Position;
        size = Size;
    }
    
    public bool Contains(Vector2 position) =>
        Raylib.CheckCollisionPointRec(position, _base);
    
    public bool Contains(Rectangle rectangle) =>
        Raylib.CheckCollisionRecs(_base, rectangle._base);
    
    /// TODO: Contains Circle
    
    public bool IsMouseOver() =>
        Raylib.CheckCollisionPointRec(Raylib.GetMousePosition(), _base);
}