﻿using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;


namespace BaseProject.GameManagement;

public enum TextureNames
{
    Cell,
    Pixel,
    Player,
}

/// <summary>
/// Contains all the textures we need to use, so we know they are in our project from the start.
/// </summary>
public static class GlobalTextures
{
    public static Dictionary<TextureNames, Texture2D> Textures { get; private set; }
    public static SpriteFont DefaultFont { get; private set; }
    //public static SpriteFont defaultFontBigger { get; private set; }

    public static void LoadContent()
    {
        ContentManager content = GameWorld.Instance.Content;
        // Load all textures
        Textures = new Dictionary<TextureNames, Texture2D>
        {
            {TextureNames.Cell, content.Load<Texture2D>("World\\16x16White") },
            {TextureNames.Pixel, content.Load<Texture2D>("World\\Pixel") },
            {TextureNames.Player, content.Load<Texture2D>("Test\\MultipleFilesAnim\\wizardRight (1)") },
        };

        // Load all fonts
        DefaultFont = content.Load<SpriteFont>("Fonts\\SmallFont");
        //defaultFontBigger = content.Load<SpriteFont>("Fonts\\FontBigger");
    }
}
