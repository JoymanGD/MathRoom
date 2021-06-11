using System;
using Microsoft.Xna.Framework;
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

        public IScene(string _name, int _id){
            Name = _name;
            ID = _id;
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