using Microsoft.Xna.Framework;
using System;

namespace Pokemon.Monogame.Models;

public readonly struct SpriteSheet : IEquatable<SpriteSheet>
{
    public int Rows { get; }

    public int Columns { get; }

    public Vector2 GridSize { get; }

    public Vector2 TileSize { get; }

    public TextureRef TextureRef { get; }


    private readonly int _size;
    private readonly Rectangle[] _sourceRectangles;

    public SpriteSheet(TextureRef textureRef, Vector2 gridSize, Vector2 tileSize)
    {
        GridSize = gridSize;
        TileSize = tileSize;

        Rows = (int)(gridSize.Y / tileSize.Y);
        Columns = (int)(gridSize.X / tileSize.X);

        TextureRef = textureRef;

        _size = Rows * Columns;
        _sourceRectangles = new Rectangle[_size];

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
}