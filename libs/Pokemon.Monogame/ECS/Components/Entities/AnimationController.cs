using Microsoft.Xna.Framework;
using Pokemon.Monogame.ECS.Components.Renderers;
using Pokemon.Monogame.Models;

namespace Pokemon.Monogame.ECS.Components.Entities;

public class AnimationController
{
    private readonly SpriteRenderer _spriteRenderer;
    
    private float _timer;
    private float _delta;
    private AnimationData? _animation;
    
    public bool IsPlaying { get; private set; }

    public int CurrentFrame { get; private set; }

    public AnimationData Animation => 
        _animation ?? default;

    public AnimationController(SpriteRenderer renderer, AnimationData startAnimation)
    {
        _animation = startAnimation;
        _timer = 0f;
        _delta = 1f / startAnimation.FramesPerSecond;
        CurrentFrame = 0;
        IsPlaying = true;
        _spriteRenderer = renderer;

        UpdateRenderer();
    }

    public void Play(in AnimationData animation)
    {
        if (animation == _animation)
            return;

        _animation = animation;
        _timer = 0f;
        _delta = 1f / animation.FramesPerSecond;
        CurrentFrame = 0;
        IsPlaying = true;
    }

    public void Stop()
    {
        IsPlaying = false;
        _timer = 0f;
        CurrentFrame = 0;
    }

    private void UpdateRenderer()
    {
        _spriteRenderer.TextureRef = Animation.SpriteSheet.TextureRef;
        _spriteRenderer.SourceRectangle = Animation.SpriteSheet.GetSourceRectangle(Animation.FrameIndices[CurrentFrame]);
    }

    public void Update(GameTime gameTime)
    {
        if (!IsPlaying)
            return;

        UpdateRenderer();

        _timer += (float)gameTime.ElapsedGameTime.TotalSeconds;

        if (_timer >= _delta)
        {
            _timer -= _delta;
            CurrentFrame = (CurrentFrame + 1) % Animation.FrameIndices.Length;
        }
    }
}
