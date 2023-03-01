using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Pokemon.Monogame.Utils;

public static class TextureUtils
{
	public static Texture2D OnePixel { get; private set; }

	public static void Initialize(GraphicsDevice graphicsDevice)
	{
		OnePixel = new Texture2D(graphicsDevice, 1, 1);
		OnePixel.SetData(new[] { Color.White });
	}

	public static Vector2 GetTextureSize(Texture2D texture) => new(texture.Width, texture.Height);
}