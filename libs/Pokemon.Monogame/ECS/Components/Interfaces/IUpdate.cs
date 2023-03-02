using Arch.Core;
using Microsoft.Xna.Framework;
using Pokemon.Monogame.ECS.Components.Renderers;

namespace Pokemon.Monogame.ECS.Components.Interfaces;

public interface IUpdate
{
    void Update(GameTime gameTime);
}
