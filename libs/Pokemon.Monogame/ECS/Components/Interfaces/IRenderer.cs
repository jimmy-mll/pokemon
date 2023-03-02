using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Pokemon.Monogame.ECS.Components.Entities;

namespace Pokemon.Monogame.ECS.Components.Interfaces;

public interface IRenderer
{
	Color Color { get; set; }

	void Render(GameScene scene, SpriteBatch spriteBatch, Position position, Scale scale);
}