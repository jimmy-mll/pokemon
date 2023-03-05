using Microsoft.Xna.Framework;

namespace Pokemon.Monogame;

public interface IVectorable<TSelf> where TSelf : IVectorable<TSelf>
{
	static abstract TSelf Zero { get; }
	static abstract TSelf One { get; }

	static abstract implicit operator Vector2(TSelf input);
	static abstract implicit operator TSelf(Vector2 input);
}