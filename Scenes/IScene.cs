using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Framework;

namespace MathRoom.Scenes
{
    public interface IScene : IDisposable
    {
        int ID { get; set; }
        string Name { get; }

        void Reset();
        void Initialize(int _sceneID);
        void Update(GameTime _gameTime);
        void Draw(SpriteBatch _spriteBatch);
    }
}