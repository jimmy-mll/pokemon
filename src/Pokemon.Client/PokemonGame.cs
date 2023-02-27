using Pokemon.Engine;
using Raylib_CsLo;

namespace Pokemon.Client;

public class PokemonGame : Game
{
	protected override void Draw()
	{
		Raylib.ClearBackground(Raylib.RED);

		base.Draw();
	}
}