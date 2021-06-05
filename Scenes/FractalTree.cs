using System.Collections;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MathRoom.Scenes
{
    public class FractalTree : IScene
    {
        public int ID { get; set; }
        public string Name { get; } = "Fractal tree";

        public void Dispose()
        {
            
        }

        public void Initialize(int _sceneID)
        {
            ID = _sceneID;
        }

        public void Update(GameTime _gameTime)
        {
            
        }

        public void Draw(SpriteBatch _spriteBatch)
        {
            
        }

        public void Reset()
        {
            throw new System.NotImplementedException();
        }
    }
}