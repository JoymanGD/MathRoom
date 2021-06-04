using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Graphics;
using MonoGame.Extended;

namespace MathRoom.Scenes
{
    public class DoublePendulum : IScene
    {
        public int ID { get; set; }
        private GraphicsDevice GraphicsDevice;

        public void Initialize(int _sceneID)
        {
            ID = _sceneID;
            GraphicsDevice = MathRoom.Instance.GraphicsDevice;
        }  

        public void Draw(SpriteBatch _spriteBatch)
        {
            //_spriteBatch.DrawCircle();
            GraphicsDevice.Clear(Color.Black);
        }

        public void Update(GameTime _gameTime)
        {
            
        }
        
        public void Dispose(){}
    }
}