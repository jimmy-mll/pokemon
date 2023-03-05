using Arch.Core;
using Arch.Core.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Pokemon.Client.Components.Scenes;
using Pokemon.Client.Services.Game;
using Pokemon.Monogame.Services.Keyboard;
using System;

namespace Pokemon.Monogame.ECS.Components.Entities;

public enum PlayerDirection
{
    Left,
    Right,
    Up,
    Down
}

public struct CharacterController
{
    private const float WalkSpeed = 150f;
    private const float RunSpeed = 250f;

    private PlayerDirection _playerDirection;

    public CharacterController()
    {
        _playerDirection = PlayerDirection.Down;
    }

    public void Update(ref Position position, AnimationController animationController, IKeyboardService kbService, GameTime gameTime)
    {
        bool isRunning = false;

        Vector2 input = Vector2.Zero;
        var moveSpeed = WalkSpeed;

        foreach (var pressedKey in Keyboard.GetState().GetPressedKeys())
        {
            var mapping = kbService.GetMappingForKey(pressedKey);

            switch (mapping)
            {
                case KeyboardMappings.Up:
                    _playerDirection = PlayerDirection.Up;
                    input.Y = -1;
                    break;

                case KeyboardMappings.Down:
                    _playerDirection = PlayerDirection.Down;
                    input.Y = 1;
                    break;

                case KeyboardMappings.Left:
                    _playerDirection = PlayerDirection.Left;
                    input.X = -1;
                    break;

                case KeyboardMappings.Right:
                    _playerDirection = PlayerDirection.Right;
                    input.X = 1;
                    break;

                case KeyboardMappings.Run:
                    moveSpeed = RunSpeed;
                    isRunning = true;
                    break;

                case KeyboardMappings.None:
                default:
                    continue;
            }
        }

        position += input * moveSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;

        if (input == Vector2.Zero) animationController?.Play(GameAnimations.PlayerIdle[_playerDirection.ToString()]);
        else
        {
            if (isRunning) animationController?.Play(GameAnimations.PlayerRun[_playerDirection.ToString()]);
            else animationController?.Play(GameAnimations.PlayerWalk[_playerDirection.ToString()]);
        }
    }
}
