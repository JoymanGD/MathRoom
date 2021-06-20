using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;

namespace MathRoom.Scenes
{
    public class Boids : IScene
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

    public struct Flockmate{
        public Vector2 Position { get; private set; }
        public Vector2 Direction { get; private set; }
        public float MaxSpeed { get; private set; }

        public Flockmate(Vector2 position, Vector2 direction, float maxSpeed){
            Position = position;
            Direction = direction;
            MaxSpeed = maxSpeed;
        }

        public void Move(Vector2 position){
            Position = position;
        }

        public void Move(Vector2 direction, float speed){
            var newPosition = Position + direction;
            Position = Vector2.Lerp(Position, newPosition, MathHelper.Clamp(speed, 0, MaxSpeed));
        }
    }
}