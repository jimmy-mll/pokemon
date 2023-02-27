using Pokemon.Engine;
using Raylib_CsLo;

namespace Pokemon.Client;

public class PokemonGame : Game
{
    protected override void Initialize()
    {
        Window.Title = "Pokemon Game !";
        Window.SetSize(500, 500);
        Window.SetPosition(200, 200);
        Window.SetOpacity(0.5f);

        base.Initialize();
    }

    protected override void Draw()
    {
        Raylib.ClearBackground(Raylib.RED);

        base.Draw();
    }
}