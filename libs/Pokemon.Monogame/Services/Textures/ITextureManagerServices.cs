using Microsoft.Xna.Framework.Graphics;

namespace Pokemon.Monogame.Services.Textures;

public interface ITextureManagerServices
{
	void AddTexture(TextureRef textureRef, Texture2D texture);

	Texture2D GetTexture(TextureRef textureRef);

	TextureRef GetTextureRef(Texture2D texture);
}