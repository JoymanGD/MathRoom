using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Framework;

namespace MathRoom.Scenes
{
    public abstract class IScene : IDisposable
    {
        public int ID { get; private set; }
        public string Name { get; private set; }
        public bool Initialized { get; private set; } = false;
        public string AdditionalInfo { get; protected set; }
        protected static GraphicsDevice GraphicsDevice { get; private set; }
        protected static ContentManager Content { get; private set; }
        protected static GraphicsDeviceManager Graphics { get; private set; }

        static IScene(){
            GraphicsDevice = MathRoom.Instance.GraphicsDevice;
            Graphics = MathRoom.Instance.Graphics;
            Content = MathRoom.Instance.Content;
        }

        public IScene(){
            Name = GetName();
            ID = MathRoom.Instance.SceneManager.Scenes.Count;
        }

        private string GetName(){
            var stringParts = this.ToString().Split('.');
            return stringParts[stringParts.Length-1];
        }

        public virtual void Initialize(){
            Initialized = true;
        }

        public virtual void Dispose(){}

        public virtual void Reset(){}

        public virtual void Update(GameTime _gameTime){}

        public virtual void Draw(SpriteBatch _spriteBatch){}
    }
}