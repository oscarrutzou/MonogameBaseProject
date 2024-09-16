﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using BaseProject.CompositPattern;
using BaseProject.GameManagement;
using BaseProject.Other;

namespace BaseProject.ComponentPattern.GUI;

public class ScalableBar : Component
{
    #region Parameters
    protected float sizeOfDrawnBar;


    protected Collider Collider;
    protected SpriteRenderer SpriteRenderer;

    protected Vector2 Position
    {
        get
        {
            return GameObject.Transform.Position;
        }
        set
        {
            GameObject.Transform.Position = value;
        }
    }


    protected Color DrawBarColor;
    protected bool FillWidth = true;

    public ScalableBar(GameObject gameObject) : base(gameObject)
    {
    }
    #endregion

    public override void Awake()
    {
        Collider = GameObject.GetComponent<Collider>();
        SpriteRenderer = GameObject.GetComponent<SpriteRenderer>();
    }

    protected void SetDrawPosOffset(int barWidth, int barHeight)
    {
        Vector2 spriteToColliderDifference = new Vector2(SpriteRenderer.Sprite.Width - barWidth, SpriteRenderer.Sprite.Height - barHeight);

        if (spriteToColliderDifference != Vector2.Zero)
            SpriteRenderer.DrawPosOffSet = -spriteToColliderDifference / 2 * GameObject.Transform.Scale;
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        // The draw method for ONLY the drawing part of the scalable bar. 
        // The logic of the contains and collision box is a part of the Collider
        Rectangle collisionBox = Collider.CollisionBox;
        Vector2 pos = new Vector2(Collider.CollisionBox.X, Collider.CollisionBox.Y);

        // The final rectangle that will be drawn
        Rectangle filledRectangle;
        float fillAmount;

        if (FillWidth)
        {
            fillAmount = sizeOfDrawnBar * collisionBox.Width;
            filledRectangle = new Rectangle((int)pos.X, (int)pos.Y, (int)fillAmount, collisionBox.Height);
        }
        else
        {
            fillAmount = sizeOfDrawnBar * collisionBox.Height;
            filledRectangle = new Rectangle((int)pos.X, (int)pos.Y, collisionBox.Width, (int)fillAmount);
        }
        // The origin 
        Vector2 origin = Vector2.Zero;
        if (Collider.CenterCollisionBox)
            origin = new Vector2(Collider.CollisionBox.Width / 2, Collider.CollisionBox.Height / 2);
        
        spriteBatch.Draw(GlobalTextures.Textures[TextureNames.Pixel],
            Position,
            filledRectangle,
            BaseFuncs.TransitionColor(DrawBarColor),
            GameObject.Transform.Rotation,
            origin,
            1f,                                    // Already have scaled from the CollisionBox
            SpriteEffects.None,
            SpriteRenderer.LayerDepth - 0.0001f); // Need to be little under the layerdepth to be under the texture
    }


}
