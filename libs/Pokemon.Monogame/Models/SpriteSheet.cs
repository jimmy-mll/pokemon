using Microsoft.Xna.Framework;
using System;

namespace Pokemon.Monogame.Models;

public readonly struct SpriteSheet : IEquatable<SpriteSheet>
{
    private readonly Rectangle[] _sourceRectangles;
    
    public int Rows { get; }

    public int Columns { get; }

    public Vector2 GridSize { get; }

    public Vector2 TileSize { get; }

    public TextureRef TextureRef { get; }

    public SpriteSheet(TextureRef textureRef, Vector2 gridSize, Vector2 tileSize)
    {
        GridSize = gridSize;
        TileSize = tileSize;

        Rows = (int)(gridSize.Y / tileSize.Y);
        Columns = (int)(gridSize.X / tileSize.X);

        TextureRef = textureRef;

        var size = Rows * Columns;
        _sourceRectangles = new Rectangle[size];

        for (var x = 0; x < Columns; x++)
        {
            for (var y = 0; y < Rows; y++)
            {
                _sourceRectangles[y * Columns + x] = new Rectangle((int)(x * tileSize.X), (int)(y * tileSize.Y), (int)tileSize.X, (int)tileSize.Y);
            }
        }
    }

    public ref Rectangle GetSourceRectangle(int index)
    {
        return ref _sourceRectangles[index];
    }

    public bool Equals(SpriteSheet other)
    {
        return Rows == other.Rows &&
               Columns == other.Columns &&
               GridSize == other.GridSize &&
               TileSize == other.TileSize &&
               TextureRef == other.TextureRef;
    }

    public override bool Equals(object obj)
    {
        return obj is SpriteSheet sheet && Equals(sheet);
    }

    public override int GetHashCode() =>
        HashCode.Combine(_sourceRectangles, Rows, Columns, GridSize, TileSize, TextureRef);

    public static bool operator ==(SpriteSheet left, SpriteSheet right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(SpriteSheet left, SpriteSheet right)
    {
        return !(left == right);
    }
}