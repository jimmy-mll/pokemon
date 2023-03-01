using Microsoft.Xna.Framework.Graphics;

namespace Pokemon.Monogame.ECS.Components.Renderers;

public interface IRenderer
{
    void Render(GameScene scene, SpriteBatch spriteBatch, Position position, Scale scale);
}