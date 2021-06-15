using System;
using System.Collections.Generic;
using MonoGame.Extended;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MathRoom.Helpers.Structs;

namespace MathRoom.Scenes
{
    public class FractalTree : IScene
    {
        GraphicsDevice GraphicsDevice;
        private Vector2 RootNode;
        public float Angle = MathF.PI / 2; //Угол поворота на 90 градусов
        public float Ang1 = MathF.PI / 4;  //Угол поворота на 45 градусов
        public float Ang2 = MathF.PI / 6;  //Угол поворота на 30 градусов

        public override void Initialize()
        {
            GraphicsDevice = MathRoom.Instance.GraphicsDevice;

            RootNode = new Vector2(400, 450);
            
            InitializeTree();
            
            base.Initialize();
        }

        private void InitializeTree(){
            Random random = new Random();
            Angle = MathF.PI / 2;
            Ang1 = MathF.PI / random.NextSingle(1f, 8f);
            Ang2 = MathF.PI / random.NextSingle(1f, 8f);
        }

        public override void Update(GameTime _gameTime)
        {
            
        }

        public override void Draw(SpriteBatch _spriteBatch)
        {
            GraphicsDevice.Clear(Color.Black);
            _spriteBatch.Begin();
            DrawTree(RootNode.X, RootNode.Y, 200, Angle, _spriteBatch);
            _spriteBatch.End();
        }

        public void DrawTree(float x, float y, float a, float Angle, SpriteBatch _spriteBatch)
        { 
            if (a > 2)
            {

                a *= 0.7f;
 
                float xnew = MathF.Round(x + a * MathF.Cos(Angle)),
                        ynew = MathF.Round(y - a * MathF.Sin(Angle));

                _spriteBatch.DrawLine(x, y, xnew, ynew, Color.White, 2);
 
                x = xnew;
                y = ynew;
 
                DrawTree(x, y, a, Angle + Ang1, _spriteBatch);
                DrawTree(x, y, a, Angle - Ang2, _spriteBatch);
            }
        }

        public override void Reset()
        {
            InitializeTree();
        }
    }
}