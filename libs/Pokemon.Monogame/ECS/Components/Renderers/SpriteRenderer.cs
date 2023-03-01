using Microsoft.Extensions.DependencyInjection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Pokemon.Monogame.ECS.Components;
using Pokemon.Monogame.Services.Textures;
using Serilog;
using Serilog.Events;

namespace Pokemon.Monogame.ECS.Components.Renderers;

public struct SpriteRenderer : IRenderer
{
    public Color Color { get; set; }
    public TextureRef TextureRef { get; set; }

    private static readonly Vector2 _originScale = new(0.5f);

    public SpriteRenderer() : this(TextureRef.None, Color.White) { }
    public SpriteRenderer(TextureRef textureRef, Color color)
    {
        TextureRef = textureRef;
        Color = color;
    }

#if DEBUG
    private bool _debugged;
#endif

    public void Render(GameScene scene, SpriteBatch spriteBatch, Position position, Scale scale)
    {
        if (TextureRef == TextureRef.None)
        {
#if DEBUG
            if (!_debugged)
            {
                Log.Write(LogEventLevel.Warning, $"'{nameof(SpriteRenderer)}' texture is referencing to 'none'.");
                _debugged = true;
            }
#endif
            return;
        }

        var texturesManager = scene.Services.GetRequiredService<ITextureManagerServices>();

        var texture = texturesManager.GetTexture(TextureRef);
        var textureSize = new Vector2(texture.Width, texture.Height); //TODO: Create extension method to get the texture size

        spriteBatch.Draw(texture, position, null, Color, 0f, _originScale * textureSize, scale, 0, 0f);
    }
}