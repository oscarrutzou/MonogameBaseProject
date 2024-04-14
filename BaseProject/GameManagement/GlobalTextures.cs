﻿using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;


namespace BaseProject.GameManagement
{
    public enum TextureNames
    {
        Cell,
        Pixel
    }

    // Dictionary of all textures
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

            };

            // Load all fonts
            DefaultFont = content.Load<SpriteFont>("Fonts\\SmallFont");
            //defaultFontBigger = content.Load<SpriteFont>("Fonts\\FontBigger");
        }
    }
}
