﻿using Pokemon.Monogame.ECS;

namespace Pokemon.Monogame.Services.Scenes;

public interface ISceneManager
{
    void SetScene<TScene>() where TScene : GameScene;

    TScene GetScene<TScene>() where TScene : GameScene;

    GameScene GetCurrentScene();
}