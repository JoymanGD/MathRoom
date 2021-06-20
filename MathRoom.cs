using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MathRoom.Scenes.SceneManager;
using MathRoom.Scenes;
using System.Collections.Generic;
using MathRoom.Patterns;
using MathRoom.Helpers;

namespace MathRoom
{
    public class MathRoom : Singleton<MathRoom>
    {
        public GraphicsDeviceManager Graphics;
        private SpriteBatch SpriteBatch;
        public DefaultSceneManager SceneManager { get; private set; }

        public MathRoom()
        {
            Graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void LoadContent()
        {
            Drawer.Initialize(Content);
            base.LoadContent();
        }

        protected override void Initialize()
        {
            // set new scenes here
            SceneManager = new DefaultSceneManager();
            SceneManager.AddScene<DoublePendulum>();
            SceneManager.AddScene<Life>();
            SceneManager.AddScene<FractalTree>();
            SceneManager.AddScene<AStar>();
            SceneManager.AddScene<Parabola>();
            SceneManager.AddScene<Boids>();

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