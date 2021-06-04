using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MathRoom.Scenes.SceneManager
{
    public class DefaultSceneManager : ISceneManager
    {
        public Dictionary<Type, IScene> Scenes { get; set; } = new Dictionary<Type, IScene>();
        public IScene CurrentScene;
        
        public DefaultSceneManager(IEnumerable<IScene> _startScenes){
            bool firstScene = false;

            int index = 0;
            
            foreach (var item in _startScenes)
            {
                item.Initialize(index);
                Scenes.Add(item.GetType(), item);
                
                if(!firstScene){
                    ChangeCurrentScene(item);
                    firstScene = true;
                }

                index++;
            }
        }

        public DefaultSceneManager(IScene _scene){
            AddScene(_scene);
            ChangeCurrentScene(_scene);
        }

        public DefaultSceneManager(){}

        public void Update(GameTime _gameTime){
            CurrentScene?.Update(_gameTime);
        }

        public void Draw(SpriteBatch _spriteBatch){
            CurrentScene?.Draw(_spriteBatch);
        }

        public void AddScene(IScene _scene){
            _scene.Initialize(Scenes.Count-1);
            Scenes.Add(_scene.GetType(), _scene);
        }

        public void RemoveScene<T>() where T:IScene{
            Scenes[typeof(T)].Dispose();
            Scenes.Remove(typeof(T));
        }

        public IScene GetScene<T>() where T:IScene{
            return Scenes[typeof(T)];
        }

        public void ChangeCurrentScene(IScene _scene){
            CurrentScene = _scene;
        }

        public void NextScene(){
            if(CurrentScene == null) return;
            
            var nextID = CurrentScene.ID + 1;

            var nextScene = Scenes.FirstOrDefault(x => x.Value.ID == nextID).Value;

            if(nextScene != null) CurrentScene = nextScene;
        }
    }
}