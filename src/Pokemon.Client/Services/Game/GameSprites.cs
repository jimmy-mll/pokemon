using Pokemon.Monogame;
using Pokemon.Monogame.Models;

namespace Pokemon.Client.Services.Game;

public static class GameSprites
{
	public static readonly TextureRef Pikachu = 0;
	public static readonly TextureRef PlayerWalk = 1;
	public static readonly TextureRef PlayerRun = 2;
}

public static class GameAnimations
{
	public static readonly AnimationGroup PlayerWalk = AnimationGroup.FromFile("Data/Animations/Player/walk_animation.json");
	public static readonly AnimationGroup PlayerRun = AnimationGroup.FromFile("Data/Animations/Player/run_animation.json");
	public static readonly AnimationGroup PlayerIdle = AnimationGroup.FromFile("Data/Animations/Player/idle_animation.json");
}