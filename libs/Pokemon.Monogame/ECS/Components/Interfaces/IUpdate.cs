using Microsoft.Xna.Framework;

namespace Pokemon.Monogame.ECS.Components.Interfaces;

public interface IUpdate
{
	void Update(GameTime gameTime);
}