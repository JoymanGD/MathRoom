using System;
using System.Collections.Generic;
using MonoGame.Extended;
using MonoGame.Extended.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using MathRoom.Helpers.Structs;

namespace MathRoom.Scenes
{
    public class Ballistic : IScene
    {
        //constant
        private const float ParabolaHeight = 200;
        private const float FloorHeight = 50;
        private const float Gravity = 9.8f;
        private readonly Size2 CannonSize = new Size2(20, 40);
        private const float StartSpeed = 12f;

        //scalable
        private Vector2 CannonPosition = new Vector2(40, 0);
        private Vector2 BallPosition = new Vector2(40, 0);
        private float CannonMovingSpeed = 2;
        private bool Thrown = false;

        //calculatable
        private Vector2 FloorStart, FloorEnd;
        private Vector2 MousePosition;
        private Vector2 Peek, Direction;
        private float Time;
        private float MiddlePoint;
        private float Max;

        public override void Initialize()
        {
            SetFloor();
            SetCanon();

            base.Initialize();
        }

        private void SetFloor(){
            var y = Graphics.PreferredBackBufferHeight - FloorHeight;
            var x1 = 0;
            var x2 = Graphics.PreferredBackBufferWidth;
            FloorStart = new Vector2(x1, y);
            FloorEnd = new Vector2(x2, y);
        }

        private void SetCanon(){
            CannonPosition = new Vector2(CannonPosition.X, FloorStart.Y - CannonSize.Height);
        }

        public override void Update(GameTime _gameTime)
        {
            var keyState = Keyboard.GetState();
            var mouseState = MouseExtended.GetState();
            
            if(keyState.IsKeyDown(Keys.D)){
                CannonPosition.X += CannonMovingSpeed;
            }
            if(keyState.IsKeyDown(Keys.A)){
                CannonPosition.X -= CannonMovingSpeed;
            }

            if(mouseState.DeltaScrollWheelValue != 0){
                CannonMovingSpeed = Math.Clamp(CannonMovingSpeed - mouseState.DeltaScrollWheelValue * .001f, 0, 10);
            }

            if(mouseState.WasButtonJustDown(MouseButton.Left)){
                BallPosition = CannonPosition;
                MousePosition = mouseState.Position.ToVector2();
                MiddlePoint = (MousePosition.X + BallPosition.X) / 2;
                Peek = new Vector2(MiddlePoint, ParabolaHeight);
                Direction = (Peek - CannonPosition).NormalizedCopy();
                Time = MathF.Abs(MousePosition.X - BallPosition.X) / StartSpeed;
                Max = MiddlePoint - BallPosition.X;
                
                Thrown = true;
            }

            if(Thrown){
                var deltaTime = (float)_gameTime.ElapsedGameTime.TotalSeconds;
                var finish = new Vector2(MousePosition.X, Graphics.PreferredBackBufferHeight - FloorHeight);

                BallPosition = Parabola(BallPosition, finish, ParabolaHeight, deltaTime);

                if(BallPosition == finish){
                    Thrown = false;
                }
            }
            else{
                BallPosition = CannonPosition;
            }

            AdditionalInfo = "Move cannon: A-D\nChange cannon moving speed: MouseWheel\nShoot: LMB\nCannon moving speed: " + MathF.Round(CannonMovingSpeed, 3);
        }

        public override void Draw(SpriteBatch _spriteBatch)
        {
            GraphicsDevice.Clear(Color.Black);
            _spriteBatch.Begin();
            _spriteBatch.DrawLine(FloorStart, FloorEnd, Color.White, 2); //floor
            _spriteBatch.DrawCircle(BallPosition + new Vector2(CannonSize.Width/2, 0), CannonSize.Width/2, 10, Color.Blue, CannonSize.Width/2); //ball
            _spriteBatch.DrawRectangle(CannonPosition, CannonSize, Color.SeaGreen, CannonSize.Width); //cannon
            _spriteBatch.DrawLine(CannonPosition, CannonPosition+Direction * 40, Color.Red, 2); //floor
            _spriteBatch.End();
        }

        public Vector2 Parabola(Vector2 start, Vector2 end, float height, float deltaTime)
        {
            var deltaX = Normalize(MiddlePoint - start.X, -Max, Max);
            var deltaY = Vector2.UnitY * deltaX;

            return Vector2.Lerp(start, end, deltaTime) - deltaY * height;
        }

        private float Normalize(float value, float min, float max){
            return (value - min) / (max - min);
        }

        public override void Reset()
        {
            
        }
    }
}