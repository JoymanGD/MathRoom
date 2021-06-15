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
        private const float FloorHeight = 50;
        private readonly Size2 CannonSize = new Size2(20, 40);

        //scalable
        private float CannonX = 40;
        private float CannonMovingSpeed = 2;

        //calculatable
        private Vector2 FloorStart, FloorEnd, Cannon;

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
            Cannon = new Vector2(CannonX, FloorStart.Y - CannonSize.Height);
        }

        public override void Update(GameTime _gameTime)
        {
            var keyState = Keyboard.GetState();
            var mouseState = MouseExtended.GetState();
            
            if(keyState.IsKeyDown(Keys.D)){
                CannonX += CannonMovingSpeed;
            }
            if(keyState.IsKeyDown(Keys.A)){
                CannonX -= CannonMovingSpeed;
            }

            if(mouseState.DeltaScrollWheelValue != 0){
                CannonMovingSpeed = Math.Clamp(CannonMovingSpeed - mouseState.DeltaScrollWheelValue * .001f, 0, 10);
            }

            Cannon.X = CannonX;

            AdditionalInfo = "Move cannon: A-D\nChange cannon moving speed: MouseWheel\nShoot: LMB\nCannon moving speed: " + MathF.Round(CannonMovingSpeed, 3);
        }

        public override void Draw(SpriteBatch _spriteBatch)
        {
            GraphicsDevice.Clear(Color.Black);
            _spriteBatch.Begin();
            _spriteBatch.DrawLine(FloorStart, FloorEnd, Color.White, 2); //floor
            _spriteBatch.DrawRectangle(Cannon, CannonSize, Color.SeaGreen, CannonSize.Width); //cannon
            _spriteBatch.End();
        }

        public override void Reset()
        {
            
        }
    }
}