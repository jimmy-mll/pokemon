using Arch.Core;
using Arch.Core.Extensions;
using Microsoft.Xna.Framework;
using Pokemon.Monogame.ECS.Components.Interfaces;
using Pokemon.Monogame.ECS.Components.Renderers;
using Pokemon.Monogame.Models;

namespace Pokemon.Monogame.ECS.Components.Entities;

public class AnimationController
{
    public bool IsPlaying => _isPlaying;

    public int CurrentFrame => _currentFrame;

    public Animation Animation => _animation ?? default;

    private float _timer;
    private float _delta;
    private bool _isPlaying;
    private int _currentFrame;
    private Animation? _animation;
    private SpriteRenderer _spriteRenderer;

    public AnimationController(SpriteRenderer renderer, Animation startAnimation)
    {
        _animation = startAnimation;
        _timer = 0f;
        _delta = 1f / startAnimation.FramesPerSecond;
        _currentFrame = 0;
        _isPlaying = true;
        _spriteRenderer = renderer;

        this.UpdateRenderer();
    }

    public void Play(in Animation animation)
    {
        if (animation == _animation)
            return;

        _animation = animation;
        _timer = 0f;
        _delta = 1f / animation.FramesPerSecond;
        _currentFrame = 0;
        _isPlaying = true;
    }

    public void Stop()
    {
        _isPlaying = false;
        _timer = 0f;
        _currentFrame = 0;
    }

    private void UpdateRenderer()
    {
        _spriteRenderer.TextureRef = Animation.Spritesheet.TextureRef;
        _spriteRenderer.SourceRectangle = Animation.Spritesheet.GetSourceRectangle(Animation.FrameIndices[_currentFrame]);
    }

    public void Update(GameTime gameTime)
    {
        if (!_isPlaying)
            return;

        UpdateRenderer();

        _timer += (float)gameTime.ElapsedGameTime.TotalSeconds;

        if (_timer >= _delta)
        {
            _timer -= _delta;
            _currentFrame = (_currentFrame + 1) % Animation.FrameIndices.Length;
        }
    }
}
