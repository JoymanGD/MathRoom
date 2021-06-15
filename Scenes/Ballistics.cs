using System;
using System.Collections.Generic;
using MonoGame.Extended;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MathRoom.Helpers.Structs;

namespace MathRoom.Scenes
{
    public class Ballistic : IScene
    {

        public override void Initialize()
        {
            
            base.Initialize();
        }

        public override void Update(GameTime _gameTime)
        {
            
        }

        public override void Draw(SpriteBatch _spriteBatch)
        {
            GraphicsDevice.Clear(Color.Black);
            _spriteBatch.Begin();
            
            _spriteBatch.End();
        }

        public override void Reset()
        {
            
        }
    }
}