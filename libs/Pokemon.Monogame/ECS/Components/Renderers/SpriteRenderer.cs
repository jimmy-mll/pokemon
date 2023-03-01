﻿using Microsoft.Extensions.DependencyInjection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Pokemon.Monogame.ECS.Components.Entities;
using Pokemon.Monogame.Services.Textures;

namespace Pokemon.Monogame.ECS.Components.Renderers;

public struct SpriteRenderer : IRenderer
{
	public Color Color { get; set; }
	public TextureRef TextureRef { get; set; }

	private static readonly Vector2 OriginScale = new(0.5f);

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
		var textureSize = new Vector2(texture.Width, texture.Height); //TODO: Create extension method to get the texture size

		spriteBatch.Draw(texture, position, null, Color, 0f, OriginScale * textureSize, scale, 0, 0f);
	}
}