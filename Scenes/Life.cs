using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;

namespace MathRoom.Scenes
{
    public class Life : IScene
    {

        #region MathPart
        //scalable
        int resolution = 4;

        //computable
        int cols;
        int rows;
        int[,] Grid;
        #endregion

        public override void Initialize()
        {
            cols = Graphics.PreferredBackBufferWidth / resolution;
            rows = Graphics.PreferredBackBufferHeight / resolution;

            Grid = new int[cols, rows];

            FillGridRandomly();
            
            base.Initialize();
        }

        private void FillGridRandomly(){
            Random random = new Random();
            for (var i = 0; i < cols; i++)
            {
                for (var j = 0; j < rows; j++)
                {
                    Grid[i,j] = random.Next(2);
                }
            }
        }

        public override void Update(GameTime _gameTime)
        {
            var nextGrid = new int[cols, rows];

            //compute next grid
            for (var i = 0; i < cols; i++)
            {
                for (var j = 0; j < rows; j++)
                {
                    int state = Grid[i,j];

                    //Count live neighbors
                    int neighbors = CountNeighbors(i,j);

                    //Live conditions
                    if(state == 0 && neighbors == 3){
                        nextGrid[i,j] = 1;
                    }
                    else if(state == 1 && (neighbors < 2 || neighbors > 3)){
                        nextGrid[i,j] = 0;
                    }
                    else{
                        nextGrid[i,j] = state;
                    }
                }
            }

            Grid = nextGrid;
        }
        
        public override void Draw(SpriteBatch _spriteBatch)
        {
            GraphicsDevice.Clear(Color.Black);

            _spriteBatch.Begin();
            for (var i = 0; i < cols; i++)
            {
                for (var j = 0; j < rows; j++)
                {
                    if(Grid[i,j] == 1){
                        int x = i * resolution;
                        int y = j * resolution;
                        _spriteBatch.DrawPoint(x, y, Color.DarkGray, resolution);
                    }
                }
            }
            _spriteBatch.End();
        }

        private int CountNeighbors(int x, int y){
            int sum = 0;
            for (var i = -1; i < 2; i++)
            {
                for (var j = -1; j < 2; j++){
                    int col = (x + i + cols) % cols;
                    int row = (y + j + rows) % rows;
                    
                    sum += Grid[col, row];
                }
            }

            sum -= Grid[x,y];

            return sum;
        }

        public override void Reset()
        {
            FillGridRandomly();
        }
    }
}