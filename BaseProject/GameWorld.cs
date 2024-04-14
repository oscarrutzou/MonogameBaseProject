using BaseProject.CommandPattern;
using BaseProject.CompositPattern;
using BaseProject.GameManagement.Scenes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace BaseProject.GameManagement
{
    public class GameWorld : Game
    {

        public static GameWorld Instance;

        public Dictionary<ScenesNames, Scene> Scenes { get; private set; }
        public Scene currentScene;
        public Camera WorldCam { get; set; }
        public Camera UiCam { get; private set; } //Static on the ui
        public static float DeltaTime { get; private set; }
        public GraphicsDeviceManager GfxManager { get; private set; }
        private SpriteBatch _spriteBatch;


        public GameWorld()
        {
            GfxManager = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            Window.Title = "Fortress Survivor";
        }

        protected override void Initialize()
        {
            ResolutionSize(1280, 720);
            //Fullscreen();
            WorldCam = new Camera(true);
            UiCam = new Camera(false);

            GlobalTextures.LoadContent();
            GlobalAnimations.LoadContent();

            GenerateScenes();
            currentScene = Scenes[ScenesNames.OscarTestScene];
            currentScene.Initialize();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
        }

        protected override void Update(GameTime gameTime)
        {
            DeltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            InputHandler.Instance.Update();
            currentScene.Update(gameTime);
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            //Draw in world objects
            _spriteBatch.Begin(sortMode: SpriteSortMode.FrontToBack, BlendState.AlphaBlend,
                SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullCounterClockwise,
                transformMatrix: WorldCam.GetMatrix());

            currentScene.DrawInWorld(_spriteBatch);
            _spriteBatch.End();

            //Draw on screen objects
            _spriteBatch.Begin(sortMode: SpriteSortMode.FrontToBack, BlendState.AlphaBlend,
                SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullCounterClockwise,
                transformMatrix: UiCam.GetMatrix());

            currentScene.DrawOnScreen(_spriteBatch);

            _spriteBatch.End();

            base.Draw(gameTime);
        }

        private void GenerateScenes()
        {
            Scenes = new Dictionary<ScenesNames, Scene>();
            Scenes[ScenesNames.GameScene] = new GameScene();
            Scenes[ScenesNames.OscarTestScene] = new OscarTestScene();
            Scenes[ScenesNames.ErikTestScene] = new ErikTestScene();
            Scenes[ScenesNames.StefanTestScene] = new StefanTestScene();
            Scenes[ScenesNames.AsserTestScene] = new AsserTestScene();
        }

        public void ResolutionSize(int width, int height)
        {
            GfxManager.HardwareModeSwitch = true;
            GfxManager.PreferredBackBufferWidth = width;
            GfxManager.PreferredBackBufferHeight = height;
            GfxManager.IsFullScreen = false;
            GfxManager.ApplyChanges();
        }

        public void Fullscreen()
        {
            GfxManager.HardwareModeSwitch = false;
            GfxManager.PreferredBackBufferWidth = GraphicsDevice.DisplayMode.Width;
            GfxManager.PreferredBackBufferHeight = GraphicsDevice.DisplayMode.Height;
            GfxManager.IsFullScreen = true;
            GfxManager.ApplyChanges();
        }

        public void Instantiate(GameObject go) => currentScene.Instantiate(go);

        public void Destroy(GameObject go) => currentScene.Destroy(go);

    }
}
