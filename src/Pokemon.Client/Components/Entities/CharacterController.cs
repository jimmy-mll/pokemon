using Arch.Core;
using Arch.Core.Extensions;
using Microsoft.Xna.Framework;
using System;

namespace Pokemon.Monogame.ECS.Components.Entities;

public struct CharacterController
{
    
    public void Update(in Position position, GameTime gameTime)
    {
        if (Vector2.DistanceSquared(position, TargetPosition) > _epsilon)
        {
            position.X += 1;
        }
    }
}
