using System;

namespace Pokemon.Monogame.Models;

public readonly struct AnimationData : IEquatable<AnimationData>
{
    public bool IsLooping { get; }
    public int FramesPerSecond { get; }
    public int[] FrameIndices { get; }
    public SpriteSheet Spritesheet { get; }

    public AnimationData(int framesPerSecond, bool isLooping, SpriteSheet spriteSheet, params int[] frameIndices)
    {
        IsLooping = isLooping;
        FramesPerSecond = framesPerSecond;
        Spritesheet = spriteSheet;
        FrameIndices = frameIndices;
    }

    public bool Equals(AnimationData other)
    {
        return IsLooping == other.IsLooping &&
               FramesPerSecond == other.FramesPerSecond &&
               FrameIndices == other.FrameIndices &&
               Spritesheet.Equals(other.Spritesheet);
    }

    public override bool Equals(object obj)
    {
        return obj is AnimationData && Equals((AnimationData)obj);
    }

    public static bool operator ==(AnimationData left, AnimationData right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(AnimationData left, AnimationData right)
    {
        return !(left == right);
    }

    public override int GetHashCode()
    {
        return IsLooping.GetHashCode() ^
               FramesPerSecond.GetHashCode() ^
               FrameIndices.GetHashCode();
    }
}
