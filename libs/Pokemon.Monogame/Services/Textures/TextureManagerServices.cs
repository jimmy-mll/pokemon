using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework.Graphics;

namespace Pokemon.Monogame.Services.Textures;

public class TextureManagerServices : ITextureManagerServices
{
	private readonly Dictionary<TextureRef, Texture2D> _refToTexDictionary = new();

	public void AddTexture(TextureRef textureRef, Texture2D texture)
	{
		if (_refToTexDictionary.ContainsKey(textureRef))
			throw new InvalidOperationException("A texture reference with the same id already exists.");

		_refToTexDictionary.Add(textureRef, texture);
	}

	public Texture2D GetTexture(TextureRef textureRef) =>
		_refToTexDictionary[textureRef];

	public TextureRef GetTextureRef(Texture2D texture)
	{
		return _refToTexDictionary.FirstOrDefault(x => x.Value == texture).Key;
	}
}