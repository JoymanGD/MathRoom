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
        private readonly Size2 CannonSize = new Size2(20, 40);
        private const float StartSpeed = 200f;

        //scalable
        private Vector2 CannonPosition = new Vector2(40, 0);
        private Vector2 BallPosition = new Vector2(40, 0);
        private float CannonMovingSpeed = 2;
        private bool Thrown = false;

        //calculatable
        private Vector2 FloorStart, FloorEnd;
        private Vector2 MousePosition;
        private Vector2 Peek, Direction, Finish;
        private float MiddlePoint;
        private float x1, x2, y0;
        private Func<float, float> Y;

        public override void Initialize()
        {
            SetFloor();
            SetCanon();
            SetBall();

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

        private void SetBall(){
            Finish = CannonPosition;
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
                Finish = new Vector2(MousePosition.X, Graphics.PreferredBackBufferHeight - FloorHeight);
                MiddlePoint = (MousePosition.X + BallPosition.X) / 2;

                x1 = BallPosition.X;
                x2 = Finish.X;
                y0 = BallPosition.Y;

                Y = (x)=>{
                    float a = ParabolaHeight / ((MiddlePoint-x1)*(MiddlePoint-x2));
                    return -a * (x-x1)*(x-x2) + y0;
                };
                
                Thrown = true;
                
                //for direction drawing
                Peek = new Vector2(MiddlePoint, ParabolaHeight);
                Direction = (Peek - CannonPosition).NormalizedCopy();
            }


            if(Thrown){
                var deltaTime = (float)_gameTime.ElapsedGameTime.TotalSeconds;

                BallPosition.X += deltaTime * StartSpeed;
                BallPosition.Y = Y(BallPosition.X);

                if(Vector2.Distance(BallPosition, Finish) < 1f){
                    Thrown = false;
                }
            }
            else{
                BallPosition = Finish;
            }

            AdditionalInfo = "Move cannon: A-D\nChange cannon moving speed: MouseWheel (" + MathF.Round(CannonMovingSpeed, 3) + ")\nShoot: LMB";
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

        public override void Reset()
        {
            
        }
    }
}