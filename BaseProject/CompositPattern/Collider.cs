using BaseProject.GameManagement;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using BaseProject.CompositPattern.Data;

namespace BaseProject.CompositPattern
{
    public class Collider : Component
    {
        private SpriteRenderer spriteRenderer;
        private Animator animator;
        private Texture2D texture;
        private int collisionWidth, collisionHeight; //If not set, use the sprite width and height
        private Vector2 offset;

        public Rectangle CollisionBox
        {
            get
            {
                int width = 0, height = 0;

                if (animator != null)
                {
                    width = collisionWidth > 0 ? collisionWidth : animator.currentAnimation.FrameDimensions;
                    height = collisionHeight > 0 ? collisionHeight : animator.currentAnimation.FrameDimensions;
                }
                else
                {
                    width = collisionWidth > 0 ? collisionWidth : spriteRenderer.Sprite.Width;
                    height = collisionHeight > 0 ? collisionHeight : spriteRenderer.Sprite.Height;
                }

                return new Rectangle
                    (
                        (int)(GameObject.Transform.Position.X - (width * GameObject.Transform.Scale.X) / 2),
                        (int)(GameObject.Transform.Position.Y - (height * GameObject.Transform.Scale.Y) / 2),
                        width * (int)GameObject.Transform.Scale.X,
                        height * (int)GameObject.Transform.Scale.X
                    );
            }
        }

        public Lazy<List<RectangleData>> rectanglesData = new Lazy<List<RectangleData>>();

        public Collider(GameObject gameObject) : base(gameObject)
        {
        }
        public override void Start()
        {
            animator = GameObject.GetComponent<Animator>();
            spriteRenderer = GameObject.GetComponent<SpriteRenderer>();
            texture = GameWorld.Instance.Content.Load<Texture2D>("Pixel");

            rectanglesData = new Lazy<List<RectangleData>>(() => CreateRectangles());

            if (spriteRenderer == null) new Exception("The collision need a spriteRenderer to work");
            var val = rectanglesData.Value;
        }

        public override void Update(GameTime gameTime)
        {
            //UpdatePixelCollider();
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            DrawRectangle(CollisionBox, spriteBatch, offset);

            //if (rectanglesData.IsValueCreated)
            //{
            //    foreach (RectangleData rectangleData in rectanglesData.Value)
            //    {
            //        DrawRectangle(rectangleData.Rectangle, spriteBatch, Vector2.Zero);
            //    }
            //}
        }

        public void SetCollisionBox(int width, int height)
        {
            collisionWidth = width;
            collisionHeight = height;
        }

        public void SetCollisionBox(int width, int height, Vector2 offset)
        {
            collisionWidth = width;
            collisionHeight = height;
            this.offset = offset;
        }

        private void DrawRectangle(Rectangle collisionBox, SpriteBatch spriteBatch, Vector2 vectorOffSet)
        {
            Vector2 colBoxPos = new Vector2(collisionBox.X, collisionBox.Y) + vectorOffSet;

            Rectangle topLine = new Rectangle((int)colBoxPos.X, (int)colBoxPos.Y, collisionBox.Width, 1);
            Rectangle bottomLine = new Rectangle((int)colBoxPos.X, (int)colBoxPos.Y + collisionBox.Height, collisionBox.Width, 1);
            Rectangle rightLine = new Rectangle((int)colBoxPos.X + collisionBox.Width, (int)colBoxPos.Y, 1, collisionBox.Height);
            Rectangle leftLine = new Rectangle((int)colBoxPos.X, (int)colBoxPos.Y, 1, collisionBox.Height);

            spriteBatch.Draw(texture, topLine, null, Color.Red, 0, Vector2.Zero, spriteRenderer.SpriteEffects, 1);
            spriteBatch.Draw(texture, bottomLine, null, Color.Red, 0, Vector2.Zero, spriteRenderer.SpriteEffects, 1);
            spriteBatch.Draw(texture, rightLine, null, Color.Red, 0, Vector2.Zero, spriteRenderer.SpriteEffects, 1);
            spriteBatch.Draw(texture, leftLine, null, Color.Red, 0, Vector2.Zero, spriteRenderer.SpriteEffects, 1);
        }

        /// <summary>
        /// Create pixel perfect collision
        /// </summary>
        /// <returns></returns>
        private List<RectangleData> CreateRectangles()
        {
            texture = GameWorld.Instance.Content.Load<Texture2D>("Pixel");
            List<Color[]> lines = new List<Color[]>();
            List<RectangleData> pixels = new List<RectangleData>();

            Vector2 scale = GameObject.Transform.Scale;
            int spriteWidth = spriteRenderer.Sprite.Width;
            int spriteHeight = spriteRenderer.Sprite.Height;

            for (int y = 0; y < spriteHeight; y++)
            {
                Color[] colors = new Color[spriteWidth];
                spriteRenderer.Sprite.GetData(0, new Rectangle(0, y, spriteWidth, 1), colors, 0, spriteRenderer.Sprite.Width);
                lines.Add(colors);
            }

            for (int y = 0; y < lines.Count; y++)
            {
                for (int x = 0; x < lines[y].Length; x++)
                {
                    if (lines[y][x].A != 0)
                    {
                        if ((x == 0) || (x == lines[y].Length) || (x > 0 && lines[y][x - 1].A == 0) || (x < lines[y].Length - 1 && lines[y][x + 1].A == 0) || (y == 0) || (y > 0 && lines[y - 1][x].A == 0) || (y < lines.Count - 1 && lines[y + 1][x].A == 0))
                        {
                            int tempX = (int)(x * scale.X);
                            int tempY = (int)(y * scale.Y);

                            if (spriteRenderer.SpriteEffects == SpriteEffects.FlipHorizontally) // Dosent work.
                            {
                                tempX = spriteWidth - tempX - 1;
                            }

                            //Dosent work with spritesheets

                            Vector2 rectanglePos = new(tempX, tempY);

                            // Calculate the pixel's position relative to the center of the sprite
                            Vector2 relativePos = rectanglePos - new Vector2(spriteWidth * scale.X / 2f, spriteHeight * scale.Y / 2f);
                            // Rotate the relative position by the sprite's rotation angle
                            float cos = MathF.Cos(GameObject.Transform.Rotation);
                            float sin = MathF.Sin(GameObject.Transform.Rotation);
                            Vector2 rotatedRelativePos = new Vector2(cos * relativePos.X - sin * relativePos.Y, sin * relativePos.X + cos * relativePos.Y);
                            // Add the sprite's position with the 
                            Vector2 pixelPos = rotatedRelativePos + new Vector2(spriteWidth / 2, spriteHeight / 2);

                            RectangleData rd = new RectangleData((int)pixelPos.X, (int)pixelPos.Y);
                            pixels.Add(rd);

                        }
                    }
                }
            }

            return pixels;
        }

        private void UpdatePixelCollider()
        {
            if (rectanglesData.IsValueCreated)
            {
                for (int i = 0; i < rectanglesData.Value.Count; i++)
                {
                    rectanglesData.Value[i].UpdatePosition(GameObject, spriteRenderer.Sprite.Width, spriteRenderer.Sprite.Height);
                }
            }
        }
    }
}
