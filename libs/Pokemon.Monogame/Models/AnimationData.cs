namespace Pokemon.Monogame.Models;

public record struct AnimationData(int FramesPerSecond, bool IsLooping, SpriteSheet SpriteSheet, params int[] FrameIndices);