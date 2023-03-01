using Microsoft.Extensions.DependencyInjection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Pokemon.Monogame.ECS.Components.Entities;
using Pokemon.Monogame.Services.Textures;

namespace Pokemon.Monogame.ECS.Components.Renderers;

public struct SpriteSheetRenderer : IRenderer
{
	public Color Color { get; set; }
	
	public TextureRef TextureRef { get; set; }
	
	public SpriteSheet SpriteSheet { get; set; }

	public SpriteSheetRenderer() : this(TextureRef.None, Color.White, default)
	{
	}

	public SpriteSheetRenderer(TextureRef textureRef, Color color, SpriteSheet sheet)
	{
		TextureRef = textureRef;
		Color = color;
		SpriteSheet = sheet;
	}
	
	public void Render(GameScene scene, SpriteBatch spriteBatch, Position position, Scale scale)
	{
		if (TextureRef == TextureRef.None)
			return;

		var texturesManager = scene.Services.GetRequiredService<ITextureService>();

		var texture = texturesManager.GetTexture(TextureRef);

		var bounds = SpriteSheet.TilePositions[SpriteSheet.TileIndexX,SpriteSheet.TileIndexY];
		
		spriteBatch.Draw(texture, position, bounds, Color, 0f, Vector2.Zero, 1f, 0, 0f);
	}
}