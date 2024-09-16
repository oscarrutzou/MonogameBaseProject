﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using BaseProject.CompositPattern;
using BaseProject.Other;
using BaseProject.CommandPattern;

namespace BaseProject.GameManagement;

public enum SceneNames
{
    OscarTestScene,
    StefanTestScene,
    ErikTestScene,
    AsserTestScene,
}

// Oscar
public abstract class Scene
{
    private List<GameObject> _newGameObjects = new List<GameObject>();
    private List<GameObject> _destroyedGameObjects = new List<GameObject>();
    protected Action OnFirstCleanUp { get; set; }
    public bool IsChangingScene { get; set; }

    protected Color CurrentTextColor { get; set; }
    public double NormalizedTransitionProgress { get; private set; }
    private double _transitionDuration { get; set; } = 0.3; // Desired duration in seconds
    private double _transitionTimer;

    public abstract void Initialize();

    /// <summary>
    /// The base update on the scene handles all the GameObjects and calls Update on them all.
    /// </summary>
    public virtual void Update()
    {
        LerpTextColor();

        CleanUp();

        if (OnFirstCleanUp != null)
        {
            OnFirstCleanUp.Invoke();
            OnFirstCleanUp = null;
        }

        foreach (GameObjectTypes type in SceneData.Instance.GameObjectLists.Keys)
        {
            if (type != GameObjectTypes.Gui && GameWorld.IsPaused) continue;

            foreach (GameObject gameObject in SceneData.Instance.GameObjectLists[type])
            {
                gameObject.Update();
            }
        }
    }

    public void Instantiate(GameObject gameObject)
    {
        _newGameObjects.Add(gameObject);
    }

    public void Destroy(GameObject go)
    {
        _destroyedGameObjects.Add(go);
    }

    public virtual void OnPlayerChanged()
    { }

    public void StartSceneChange()
    {
        IsChangingScene = true;
        NormalizedTransitionProgress = 0; // Start with full opacity
        _transitionTimer = 0;
    }

    private void LerpGameObjects()
    {
        // Maybe a shader offset, that just tweaks the values for the gameobjects? 
        _transitionTimer += GameWorld.DeltaTime;

        NormalizedTransitionProgress = Math.Clamp(_transitionTimer / _transitionDuration, 0, 1);

        foreach (GameObjectTypes type in SceneData.Instance.GameObjectLists.Keys)
        {
            // The cells shouldnt be turned transparent, since that allows the player to see whats under them.
            if (type == GameObjectTypes.Cell) continue;

            foreach (GameObject gameObject in SceneData.Instance.GameObjectLists[type])
            {
                SpriteRenderer sr = gameObject.GetComponent<SpriteRenderer>();
                if (sr == null) continue;

                if (sr.StartColor == Color.Transparent)
                    sr.StartColor = sr.Color;

                sr.Color = Color.Lerp(sr.StartColor, Color.Transparent, (float)NormalizedTransitionProgress);
            }
        }
    }

    
    private void LerpTextColor()
    {
        CurrentTextColor = BaseFuncs.TransitionColor(TextColor);
    }

    public static Color TextColor = Color.White;
    public static Color BGColor = Color.DarkGray;

    public void OnSceneChange()
    {
        LerpGameObjects();

        // If the progrss of the lerp has not finnished, we wont change scenes
        if (NormalizedTransitionProgress != 1) return;

        IsChangingScene = false;
        
        OnFirstCleanUp = null;

        InputHandler.Instance.RemoveAllExeptBaseCommands();
    }

    /// <summary>
    /// <para>The method adds the newGameobjects to different lists, and calls the Awake and Start on the Objects, so the objects starts properly.</para>
    /// <para>It also removes the gameobjects if there are any</para>
    /// </summary>
    private void CleanUp()
    {
        if (_newGameObjects.Count == 0 && _destroyedGameObjects.Count == 0) return; //Shouldnt run since there is no new changes

        for (int i = 0; i < _newGameObjects.Count; i++)
        {
            AddToCategory(_newGameObjects[i]);
            _newGameObjects[i].Awake();
            _newGameObjects[i].Start();
        }
        for (int i = 0; i < _destroyedGameObjects.Count; i++)
        {
            RemoveFromCategory(_destroyedGameObjects[i]);
        }

        _newGameObjects.Clear();
        _destroyedGameObjects.Clear();
    }

    /// <summary>
    /// Adds the gameObject to the correct list based on the gameobjects type.
    /// </summary>
    /// <param name="gameObject"></param>
    private void AddToCategory(GameObject gameObject)
    {
        // We know the Lists gets made and only gets Cleared when changing scene,
        // so we can assume that the lists are already there.
        // If something went wrong, the compiler will send a error anyway, so we dont need a extra check, for small projects like these.
        SceneData.Instance.GameObjectLists[gameObject.Type].Add(gameObject);

        // Checks if the gameobject has any effects on it.
        SceneData.Instance.AddGameObject(gameObject);
    }

    /// <summary>
    /// Removes the gameObject from the respective list that they are in.
    /// </summary>
    /// <param name="gameObject"></param>
    private void RemoveFromCategory(GameObject gameObject)
    {
        SceneData.Instance.GameObjectLists[gameObject.Type].Remove(gameObject);

        // Removes the gameobject if its in the list
        SceneData.Instance.RemoveGameObject(gameObject);
    }

    /// <summary>
    /// Draws everything that is not the Gui GameObjectType with the WorldCam.
    /// </summary>
    /// <param name="spriteBatch"></param>
    private Dictionary<Effect, List<GameObject>> _effectGroups = new Dictionary<Effect, List<GameObject>>();

    public virtual void DrawInWorld(SpriteBatch spriteBatch)
    {
        bool usesSimpleDrawCall = false;

        foreach (var spriteRenderer in SceneData.Instance.GetSortedGameObjects())
        {
            if (spriteRenderer.GameObject.ShaderEffect == null)
            {
                if (!usesSimpleDrawCall)
                {
                    spriteBatch.Begin(sortMode: SpriteSortMode.FrontToBack, BlendState.AlphaBlend,
                        SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullCounterClockwise,
                        transformMatrix: GameWorld.Instance.WorldCam.GetMatrix());
                    usesSimpleDrawCall = true;
                }
                spriteRenderer.GameObject.Draw(spriteBatch);
            }
            else
            {
                if (usesSimpleDrawCall)
                {
                    spriteBatch.End();
                    usesSimpleDrawCall = false;
                }
                spriteBatch.Begin(sortMode: SpriteSortMode.FrontToBack, BlendState.AlphaBlend,
                    SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullCounterClockwise,
                    transformMatrix: GameWorld.Instance.WorldCam.GetMatrix(), effect: spriteRenderer.GameObject.ShaderEffect);
                spriteRenderer.GameObject.Draw(spriteBatch);
                spriteBatch.End();
            }
        }

        if (usesSimpleDrawCall) spriteBatch.End();
    }


    /// <summary>
    /// Draws the Gui GameObjects on the UiCam.
    /// </summary>
    /// <param name="spriteBatch"></param>
    public virtual void DrawOnScreen(SpriteBatch spriteBatch)
    {
        //Draw on screen objects. Use pixel perfect and a stationary UiCam that dosent move around
        DrawMouse(spriteBatch);

        // Draw all Gui GameObjects in the active scene.
        foreach (GameObject gameObject in SceneData.Instance.GameObjectLists[GameObjectTypes.Gui])
        {
            gameObject.Draw(spriteBatch);
        }
    }

    private void DrawMouse(SpriteBatch spriteBatch)
    {
        InputHandler.Instance.MouseGo?.Draw(spriteBatch);
        //spriteBatch.DrawString(GlobalTextures.DefaultFont, $"FPS: {GameWorld.Instance.AvgFPS}", GameWorld.Instance.UiCam.TopLeft, Color.DarkGreen);
    }


    public virtual void DrawSceenColor()
    {
        GameWorld.Instance.GraphicsDevice.Clear(BGColor);
    }
}