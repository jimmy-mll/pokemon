using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Pokemon.Monogame.ECS.Components.Renderers;

public interface IRenderer
{
    Color Color { get; set; }
    void Render(GameScene scene, SpriteBatch spriteBatch, Position position, Scale scale);
}