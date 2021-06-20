using System;
using MonoGame.Extended;
using MonoGame.Extended.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

namespace MathRoom.Scenes
{
    public class Parabola : IScene
    {
        //constant
        private const float CannonYOffset = 84;

        //scalable
        private float ParabolaHeight = 200;
        private float FloorHeight = 50;
        private Vector2 CannonPosition = new Vector2(40, 0);
        private float CannonMovingSpeed = 8;
        private float CannonRotation = 0;
        private Vector2 CannonOffset;
        private Vector2 BallPosition = new Vector2(40, 0);
        private float BallRadius;
        private bool Thrown = false;

        //calculatable
        private Texture2D Cannon;
        private float StartSpeed;
        private Vector2 FloorStart, FloorEnd;
        private Vector2 MousePosition;
        private Vector2 Peek, Finish;
        private float MiddlePoint;
        private Func<float, float> ParabolaY;

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
            Cannon = Content.Load<Texture2D>("Images/Cannon");
            CannonOffset = new Vector2(Cannon.Width / 2, CannonYOffset);
            CannonRotation = 0;
            CannonPosition = new Vector2(CannonPosition.X, FloorStart.Y);
        }

        private void SetBall(){
            Finish = CannonPosition;
            BallPosition = Finish;

            BallRadius = 5;
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
                ParabolaHeight = Math.Clamp(ParabolaHeight - mouseState.DeltaScrollWheelValue * .1f, 50, 400);
            }

            Rotate(mouseState);

            if(mouseState.WasButtonJustDown(MouseButton.Left)){
                BallPosition = CannonPosition;
                Finish = new Vector2(MousePosition.X, Graphics.PreferredBackBufferHeight - FloorHeight);
                StartSpeed =  MousePosition.X - BallPosition.X;
                
                MiddlePoint = (MousePosition.X + BallPosition.X) / 2;
                var start = BallPosition.X;
                var end = MousePosition.X;
                var y0 = BallPosition.Y;

                ParabolaY = (x)=>{
                    return GetParabolaY(x, start, end, y0, MiddlePoint);
                };
                
                Thrown = true;
                
                //for direction drawing
            }


            if(Thrown){
                var deltaTime = (float)_gameTime.ElapsedGameTime.TotalSeconds;

                BallPosition.X += deltaTime * StartSpeed;
                BallPosition.Y = ParabolaY(BallPosition.X);

                if(Vector2.Distance(BallPosition, Finish) < 1f){
                    Thrown = false;
                }
            }
            else{
                BallPosition = Finish;
            }

            AdditionalInfo = "Move cannon: A-D\nChange height: MouseWheel (" + MathF.Round(ParabolaHeight, 3) + ")\nShoot: LMB";
        }

        public override void Draw(SpriteBatch _spriteBatch)
        {
            GraphicsDevice.Clear(Color.Black);
            _spriteBatch.Begin();
            _spriteBatch.DrawLine(FloorStart, FloorEnd, Color.White, 2); //floor
            DrawTrajectory(_spriteBatch); // trajectory
            _spriteBatch.DrawCircle(BallPosition, BallRadius, 10, Color.SkyBlue, BallRadius); //ball
            _spriteBatch.Draw(Cannon, CannonPosition, null, Color.White, CannonRotation, CannonOffset, Vector2.One/2, SpriteEffects.None, 0); //cannon
            _spriteBatch.End();
        }

        public override void Reset()
        {
            
        }

        private void DrawTrajectory(SpriteBatch spriteBatch){            
            var start = CannonPosition.X;
            var x = start;
            var end = MousePosition.X;
            var middlePoint = (start + end) / 2;
            
            while(MathHelper.Distance(x, end) > 0.1f){
                var y = GetParabolaY(x, start, end, CannonPosition.Y, middlePoint);
                var pointPos = new Vector2(x, y);
                spriteBatch.DrawPoint(x, y, Color.White, 2);
                x = MathHelper.Lerp(x, end, .1f);
            }
        }

        private float GetParabolaY(float x, float x1, float x2, float y0, float middlePoint){
            float a = ParabolaHeight / ((middlePoint-x1)*(middlePoint-x2));
            return -a * (x-x1)*(x-x2) + y0;
        }

        private void Rotate(MouseStateExtended mouseState){
            MousePosition = mouseState.Position.ToVector2();
            Peek = new Vector2((MousePosition.X + CannonPosition.X) / 2, ParabolaHeight);
            var x = MathHelper.Lerp(CannonPosition.X, MousePosition.X, .01f);
            var middlePoint = (CannonPosition.X + MousePosition.X) / 2;
            var endPosition = new Vector2(x, GetParabolaY(x, CannonPosition.X, MousePosition.X, CannonPosition.Y, middlePoint));

            CannonRotation = GetRotation(CannonPosition, endPosition);
        }

        private float GetRotation(Vector2 from, Vector2 to){
            var a = Vector2.UnitY;
            
            var b = from - to;
            b.Normalize();

            var angle = MathF.Acos(Vector2.Dot(a, b)/(a.Length() * b.Length())) * MathF.Sign(to.X - from.X);

            return angle;
        }
    }
}