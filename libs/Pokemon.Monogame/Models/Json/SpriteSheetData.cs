using Microsoft.Xna.Framework;
using Pokemon.Monogame.Models.Json.Interfaces;

namespace Pokemon.Monogame.Models.Json;

public struct SpriteSheetData : IJsonData<SpriteSheet>
{
    public int TextureReference { get; set; }

    public int GridWidth { get; set; }
    public int GridHeight { get; set; }

    public int TileWidth { get; set; }
    public int TileHeight { get; set; }

    public SpriteSheet GetValue()
    {
        return new SpriteSheet(TextureReference, new Vector2(GridWidth, GridHeight), new Vector2(TileWidth, TileHeight));
    }
}
