using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;

namespace MathRoom.Scenes
{
    public class DoublePendulum : IScene
    {
        private SpriteBatch TrailSpriteBatch;
        private RenderTarget2D TrailRenderTarget;

        #region MathPart
        //scalable
        Vector2 p0 = new Vector2(400,200);
        float l1 = 100;
        float l2 = 100;
        float m1 = 15;
        float m2 = 10;
        float g = 10;
        
        //computable
        float a1;
        float a2;
        float a1_v;
        float a2_v;
        float a1_a;
        float a2_a;
        Vector2 p1;
        Vector2 p2;
        Vector2 p_cash;
        int width, height;

        //constant
        const float speedModifier = 0.05f;
        #endregion

        public override void Initialize()
        {
            TrailSpriteBatch = new SpriteBatch(GraphicsDevice);
            width = Graphics.PreferredBackBufferWidth;
            height = Graphics.PreferredBackBufferHeight;
            TrailRenderTarget = GetRenderTarget();
            RandomizeAngles();
            
            base.Initialize();
        }

        RenderTarget2D GetRenderTarget(){
            var rt = new RenderTarget2D(GraphicsDevice, width, height, false, SurfaceFormat.Vector4, DepthFormat.Depth24, 0, RenderTargetUsage.PreserveContents);
            
            return rt;
        }

        float GetRandomAngle(){
            Random random = new Random();
            return random.NextAngle();
        }

        void RandomizeAngles(){
            a1 = GetRandomAngle();
            a2 = GetRandomAngle();
        }

        public override void Update(GameTime _gameTime)
        {
            a1_a = (-g*(2*m1 + m2)*Sin(a1)-m2*g*Sin(a1-2*a2)-2*Sin(a1-a2)*m2*(a2_v*a2_v*l2+a1_v*a1_v*l1*Cos(a1-a2))) / (l1*(2*m1+m2-m2*Cos(2*a1-2*a2)));
            a2_a = (2*Sin(a1-a2)*(a1_v*a1_v*l1*(m1+m2)+g*(m1+m2)*Cos(a1)+a2_v*a2_v*l2*m2*Cos(a1-a2))) / (l2*(2*m1+m2-m2*Cos(2*a1-2*a2)));
            a1_v += a1_a * speedModifier;
            a2_v += a2_a * speedModifier;
            a1 += a1_v;
            a2 += a2_v;

            p1 = new Vector2(p0.X + l1 * MathF.Sin(a1), p0.Y + l1 * MathF.Cos(a1));
            p2 = new Vector2(p1.X + l2 * MathF.Sin(a2), p1.Y + l2 * MathF.Cos(a2));
        }

        public override void Draw(SpriteBatch _spriteBatch)
        {
            GraphicsDevice.SetRenderTarget(TrailRenderTarget);
                _spriteBatch.Begin(blendState: BlendState.AlphaBlend);
                    if(p_cash != Vector2.Zero){
                        _spriteBatch.DrawLine(p_cash, p2, Color.LightBlue * .2f, 1,1);
                        //_spriteBatch.DrawPoint(p_cash, Color.LightBlue, 1,1);
                    }
                    p_cash = p2;
                _spriteBatch.End();
            GraphicsDevice.SetRenderTarget(null);
            
            GraphicsDevice.Clear(Color.Black);

            _spriteBatch.Begin();
                _spriteBatch.Draw(TrailRenderTarget, new Rectangle(0,0,width, height), Color.White);
            _spriteBatch.End();
                    
            _spriteBatch.Begin();
                //balls
                _spriteBatch.DrawCircle(p1, m1, 30, Color.White, m1); //first ball
                _spriteBatch.DrawCircle(p2, m2, 30, Color.White, m2); //second ball

                //lines
                _spriteBatch.DrawLine(p0, p1, Color.White); //from start to first ball
                _spriteBatch.DrawLine(p1, p2, Color.White); //from first ball to second ball
            _spriteBatch.End();
        }

        private float Sin(float _value){
            return MathF.Sin(_value);
        }

        private float Cos(float _value){
            return MathF.Cos(_value);
        }
        
        public override void Reset()
        {
            a1_v = 0;
            a2_v = 0;
            a1_a = 0;
            a2_a = 0;
            p_cash = Vector2.Zero;
            RandomizeAngles();
            p1 = new Vector2(p0.X + l1 * MathF.Sin(a1), p0.Y + l1 * MathF.Cos(a1));
            p2 = new Vector2(p1.X + l2 * MathF.Sin(a2), p1.Y + l2 * MathF.Cos(a2));
            GraphicsDevice.SetRenderTarget(TrailRenderTarget);
            GraphicsDevice.Clear(Color.Black);
            GraphicsDevice.SetRenderTarget(null);
        }
    }
}