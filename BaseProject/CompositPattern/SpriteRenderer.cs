using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using BaseProject.GameManagement;

namespace BaseProject.CompositPattern
{
    public enum LAYERDEPTH
    {
        Default,
        WorldBackground,
        Enemies,
        Player,
        WorldForeground,
        UI,
        Button,
        Text,
        CollisionDebug,
    }

    public class SpriteRenderer : Component
    {
        public Texture2D Sprite { get; set; }
        public Color Color { get; set; } = Color.White;

        public Vector2 Origin { get; set; }
        public bool IsCentered = true;
        private float LayerDepth;
        public LAYERDEPTH LayerName { get; private set; } = LAYERDEPTH.Default;
        public SpriteEffects SpriteEffects { get; set; } = SpriteEffects.None;

        public Rectangle SourceRectangle;
        public bool UsingAnimation;
        private Animator animator;

        public SpriteRenderer(GameObject gameObject) : base(gameObject)
        {

        }

        public override void Start()
        {
            animator = GameObject.GetComponent<Animator>();
        }

        public void SetLayerDepth(LAYERDEPTH layerName)
        {
            LayerName = layerName;
            LayerDepth = (float)LayerName / (Enum.GetNames(typeof(LAYERDEPTH)).Length - 1);
        }

        private Vector2 drawPos;
        public override void Draw(SpriteBatch spriteBatch)
        {
            if (Sprite == null) return;

            Origin = IsCentered ? new Vector2(Sprite.Width / 2, Sprite.Height / 2) : Vector2.Zero;

            drawPos = GameObject.Transform.Position;

            if (animator != null)
            {
                drawPos += new Vector2(animator.MaxFrames * animator.currentAnimation.FrameDimensions * GameObject.Transform.Scale.X / 2 - (float)(animator.currentAnimation.FrameDimensions * 2), 0);
            }
            //Draws the sprite, and if there is a sourcerectangle set, then it uses that.
            spriteBatch.Draw(Sprite, drawPos, SourceRectangle == Rectangle.Empty ? null : SourceRectangle, Color, GameObject.Transform.Rotation, Origin, GameObject.Transform.Scale, SpriteEffects, LayerDepth);


        }

        public void SetSprite(TextureNames spriteName)
        {
            UsingAnimation = false;
            Sprite = GlobalTextures.Textures[spriteName];
        }


    }
}
