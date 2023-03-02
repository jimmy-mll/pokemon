using Microsoft.Extensions.DependencyInjection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Pokemon.Monogame.ECS.Components.Entities;
using Pokemon.Monogame.ECS.Components.Interfaces;
using Pokemon.Monogame.Services.Textures;

namespace Pokemon.Monogame.ECS.Components.Renderers;

public class SpriteRenderer : IRenderer
{
	private static readonly Vector2 OriginScale = new(0.5f);
	public Color Color { get; set; }
	public TextureRef TextureRef { get; set; }
	public Rectangle? SourceRectangle { get; set; }

	public SpriteRenderer() : this(TextureRef.None, Color.White)
	{
	}

	public SpriteRenderer(TextureRef textureRef, Color color)
	{
		TextureRef = textureRef;
		Color = color;
	}

	public void Render(GameScene scene, SpriteBatch spriteBatch, Position position, Scale scale)
	{
		if (TextureRef == TextureRef.None)
			return;

		var texturesManager = scene.Services.GetRequiredService<ITextureService>();

		var texture = texturesManager.GetTexture(TextureRef);
		var textureSize = SourceRectangle is null
			? new Vector2(texture.Width, texture.Height)
			: new Vector2(SourceRectangle.Value.Width, SourceRectangle.Value.Height);

		spriteBatch.Draw(texture, position, SourceRectangle, Color, 0f, OriginScale * textureSize, scale, 0, 0f);
	}
}