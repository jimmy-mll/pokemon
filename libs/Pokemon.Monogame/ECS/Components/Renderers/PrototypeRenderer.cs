using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Pokemon.Monogame.Utils;

namespace Pokemon.Monogame.ECS.Components.Renderers;

public struct PrototypeRenderer : IRenderer
{
	public int Width { get; set; }
	public int Height { get; set; }
	public Color Color { get; set; }

	private static readonly Vector2 _origin = new(0.5f);

	public PrototypeRenderer() : this(100, 100, Color.White)
	{
	}

	public PrototypeRenderer(int width, int height, Color color)
	{
		Width = width;
		Height = height;
		Color = color;
	}

	public void Render(GameScene scene, SpriteBatch spriteBatch, Position position, Scale scale)
	{
		spriteBatch.Draw(TextureUtils.OnePixel, position, null, Color, 0f, _origin, new Vector2(Width, Height) * scale, 0, 0f);
	}
}