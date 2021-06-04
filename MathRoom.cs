using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MathRoom.Scenes.SceneManager;
using MathRoom.Scenes;
using System.Collections.Generic;
using MathRoom.Patterns;

namespace MathRoom
{
    public class MathRoom : Singleton<MathRoom>
    {
        public GraphicsDeviceManager Graphics;
        private SpriteBatch SpriteBatch;
        private DefaultSceneManager SceneManager;

        public MathRoom()
        {
            Graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // set new scenes here
            SceneManager = new DefaultSceneManager(new List<IScene>{ 
                new DoublePendulum()
            });

            SpriteBatch = new SpriteBatch(GraphicsDevice);

            base.Initialize();
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            SceneManager.Update(gameTime);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            SceneManager.Draw(SpriteBatch);

            base.Draw(gameTime);
        }
    }
}
