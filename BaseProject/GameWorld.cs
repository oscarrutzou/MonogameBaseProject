using BaseProject.CommandPattern;
using BaseProject.CompositPattern;
using BaseProject.GameManagement.Scenes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace BaseProject.GameManagement;

public class GameWorld : Game
{
    #region Properties
    public static GameWorld Instance;
    public static bool DebugAndCheats = false;

    public Dictionary<SceneNames, Scene> Scenes { get; private set; }
    public Scene CurrentScene { get; private set; }
    public SceneNames? NextScene { get; private set; } = null;
    public Camera WorldCam { get; private set; } 
    public Camera UiCam { get; private set; } //Static on the ui 
    public static double DeltaTime { get; private set; }
    public static float DeltaTimeF { get { return (float)DeltaTime; }}
    public static bool IsPaused { get; set; } = false;
    public double GameWorldSpeed { get; private set; } = 1.0f;
    public float AvgFPS { get; private set; }
    public GraphicsDeviceManager GfxManager { get; private set; }
    private SpriteBatch _spriteBatch;
    public int DisplayWidth => GfxManager.PreferredBackBufferWidth;
    public int DisplayHeight => GfxManager.PreferredBackBufferHeight;
    #endregion
    public GameWorld()
    {
        GfxManager = new GraphicsDeviceManager(this);
        Content.RootDirectory = "A-Content";
        IsMouseVisible = true;
        Window.Title = "Base Project";
    }

    protected override void Initialize()
    {
        
        IsFixedTimeStep = false;
        GfxManager.SynchronizeWithVerticalRetrace = true;

        ResolutionSize(1280, 720);
        GlobalTextures.LoadContent();
        GlobalAnimations.LoadContent();

        SceneData.Instance.GenereateGameObjectDicionary();
        //Fullscreen();
        WorldCam = new Camera();
        UiCam = new Camera();


        GenerateScenes();
        CurrentScene = Scenes[SceneNames.OscarTestScene];
        CurrentScene.Initialize();

        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
    }

    protected override void Update(GameTime gameTime)
    {
        DeltaTime = gameTime.ElapsedGameTime.TotalSeconds;

        UpdateFPS(gameTime);
        InputHandler.Instance.Update(); //Updates our input, so its not each scene that have to handle the call.

        if (!IsActive) return;
        GlobalSounds.MusicUpdate(); // Updates the Music in the game, not SFX

        CurrentScene.Update();
        HandleSceneChange(); // Goes to the next scene

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        CurrentScene.DrawSceenColor();


        CurrentScene.DrawInWorld(_spriteBatch);

        //Draw on screen objects. Use pixel perfect and a stationary UiCam that dosent move around
        _spriteBatch.Begin(sortMode: SpriteSortMode.FrontToBack, BlendState.AlphaBlend,
            SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullCounterClockwise,
            transformMatrix: UiCam.GetMatrix());

        CurrentScene.DrawOnScreen(_spriteBatch);
        _spriteBatch.End();


        base.Draw(gameTime);
    }

    /// <summary>
    /// Generates the scenes that can be used in the project.
    /// </summary>
    private void GenerateScenes()
    {
        Scenes = new Dictionary<SceneNames, Scene>();
        Scenes[SceneNames.OscarTestScene] = new OscarTestScene();
        Scenes[SceneNames.ErikTestScene] = new ErikTestScene();
        Scenes[SceneNames.StefanTestScene] = new StefanTestScene();
        Scenes[SceneNames.AsserTestScene] = new AsserTestScene();
    }

    public void ResolutionSize(int width, int height)
    {
        GfxManager.HardwareModeSwitch = true;
        GfxManager.PreferredBackBufferWidth = width;
        GfxManager.PreferredBackBufferHeight = height;
        GfxManager.IsFullScreen = false;
        GfxManager.ApplyChanges();
    }

    /// <summary>
    /// Sets the project to fullscreen on Load
    /// </summary>
    public void Fullscreen()
    {
        GfxManager.HardwareModeSwitch = false;
        GfxManager.PreferredBackBufferWidth = GraphicsDevice.DisplayMode.Width;
        GfxManager.PreferredBackBufferHeight = GraphicsDevice.DisplayMode.Height;
        GfxManager.IsFullScreen = true;
        GfxManager.ApplyChanges();
    }

    private void UpdateFPS(GameTime gameTime)
    {
        float fps = 0;
        if (gameTime.ElapsedGameTime.TotalMilliseconds > 0)
            fps = (float)Math.Round(1000 / (gameTime.ElapsedGameTime.TotalMilliseconds), 1);

        //Set _avgFPS to the first fps value when started.
        if (AvgFPS < 0.01f) AvgFPS = fps;

        //Average over 20 frames
        AvgFPS = AvgFPS * 0.95f + fps * 0.05f;
    }

    /// <summary>
    /// Adds the GameObject to the CurrentScene
    /// </summary>
    /// <param name="go"></param>
    public void Instantiate(GameObject go) => CurrentScene.Instantiate(go);
    /// <summary>
    /// Removes the GameObject from the CurrrentScene
    /// </summary>
    /// <param name="go"></param>
    public void Destroy(GameObject go) => CurrentScene.Destroy(go);


    public void ChangeScene(SceneNames sceneName) => NextScene = sceneName;
    /// <summary>
    /// A method to prevent changing in the GameObject lists while its still inside the Update
    /// </summary>
    private void HandleSceneChange()
    {
        if (NextScene == null || Scenes[NextScene.Value] == null) return;

        if (!CurrentScene.IsChangingScene)
        {
            CurrentScene.StartSceneChange();
        }

        //SaveData.SetBaseValues();
        CurrentScene.OnSceneChange(); // Removes commands and more

        // Wait for current scene to turn down alpha on objects
        if (CurrentScene.IsChangingScene) return;

        SceneData.Instance.DeleteAllGameObjects();

        WorldCam.Position = Vector2.Zero; // Resets world cam position
        CurrentScene = Scenes[NextScene.Value]; // Changes to new scene
        CurrentScene.Initialize(); // Starts the new scene

        IsPaused = false;
        NextScene = null;
    }
}
