using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MathRoom.Scenes.SceneManager
{
    public interface ISceneManager
    {
        Dictionary<Type, IScene> Scenes { get; set; }

        void ChangeCurrentScene(IScene _scene);

        void NextScene();
        
        void RemoveScene<T>() where T:IScene;

        void AddScene(IScene _scene);

        IScene GetScene<T>() where T:IScene;

        void Draw(SpriteBatch _spriteBatch);

        void Update(GameTime _gameTime);
    }
}