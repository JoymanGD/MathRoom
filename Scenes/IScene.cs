using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Framework;

namespace MathRoom.Scenes
{
    public interface IScene : IDisposable
    {
        int ID { get; set; }
        void Initialize(int _sceneID);
        void Draw(SpriteBatch _spriteBatch);
        void Update(GameTime _gameTime);
    }
}