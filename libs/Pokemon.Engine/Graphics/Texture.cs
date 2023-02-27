using System.Numerics;
using Raylib_CsLo;

namespace Pokemon.Engine.Graphics;

public sealed class Texture : IDisposable
{
    private static readonly Color Tint;
    
    private readonly Raylib_CsLo.Texture _base;
    
    public uint Id =>
        _base.id;
    
    public int Width => 
        _base.width;
    
    public int Height =>
        _base.height;
    
    public int MipMaps =>
        _base.mipmaps;

    public int Format =>
        _base.format;
    
    public PixelFormat PixelFormat =>
        _base.format_;

    static Texture() =>
        Tint = Raylib.WHITE;

    public Texture(string path) =>
        _base = Raylib.LoadTexture(path);
    
    public void Deconstruct(out uint id, out int width, out int height, out int mipmaps, out int format, out PixelFormat pixelFormat)
    {
        id = Id;
        width = Width;
        height = Height;
        mipmaps = MipMaps;
        format = Format;
        pixelFormat = PixelFormat;
    }

    public void Draw(Vector2 position, Color? tint = null) =>
        Raylib.DrawTextureV(_base, position, tint ?? Tint);
    
    
    
    public void Dispose() =>
        Raylib.UnloadTexture(_base);
}