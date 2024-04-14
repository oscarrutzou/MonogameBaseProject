using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System;
using BaseProject.GameManagement;

namespace BaseProject.CompositPattern
{
    internal class Animator : Component
    {
        public int CurrentIndex { get; private set; }
        private float timeElapsed;
        private SpriteRenderer spriteRenderer;
        private Dictionary<AnimNames, Animation> animations = new Dictionary<AnimNames, Animation>();
        public Animation currentAnimation { get; private set; }

        public bool IsLooping { get; set; }
        private bool hasPlayedAnim;

        private float frameDuration;
        public int MaxFrames;

        public Animator(GameObject gameObject) : base(gameObject)
        {
        }

        public override void Start()
        {
            spriteRenderer = GameObject.GetComponent<SpriteRenderer>();
            if (spriteRenderer == null)
                throw new Exception($"No spriteRenderer on gameObject, and therefore its not possible to Animate");

        }

        public override void Update(GameTime gameTime)
        {
            if (currentAnimation == null) return;

            //if (IsLooping && hasPlayedAnim) return; // Already have played the animation once, so it can stop.
            timeElapsed += GameWorld.DeltaTime;

            if (currentAnimation.UseSpriteSheet) UpdateSpriteSheet();
            else UpdateIndividualFrames();
        }


        private void UpdateIndividualFrames()
        {
            if (timeElapsed > frameDuration)
            {
                //Set new frame
                timeElapsed = 0;
                CurrentIndex = (CurrentIndex + 1) % currentAnimation.Sprites.Length;

                if (CurrentIndex == 0)
                {
                    if (IsLooping) hasPlayedAnim = true; //So it stops the looping, dont need to be set to true since it dosent get used by other parts

                    currentAnimation.OnAnimationDone?.Invoke();
                }
            }

            spriteRenderer.Sprite = currentAnimation.Sprites[CurrentIndex];
        }

        private void UpdateSpriteSheet()
        {

            if (timeElapsed > frameDuration)
            {
                //Set new frame
                timeElapsed = 0;
                CurrentIndex = (CurrentIndex + 1) % MaxFrames; //So it turns to 0 when it goes over maxframes

                if (CurrentIndex == 0)
                {
                    if (IsLooping) hasPlayedAnim = true; //So it stops the looping, dont need to be set to true since it dosent get used by other parts

                    currentAnimation.OnAnimationDone?.Invoke();
                }
            }

            spriteRenderer.SourceRectangle.X = CurrentIndex * currentAnimation.FrameDimensions;
        }

        public void AddAnimation(Animation animation) => animations.Add(animation.Name, animation);

        public void PlayAnimation(AnimNames animationName)
        {
            try
            {
                currentAnimation = animations[animationName];
                spriteRenderer.UsingAnimation = true;

                if (currentAnimation.UseSpriteSheet)
                {
                    spriteRenderer.SourceRectangle = currentAnimation.SourceRectangle;
                    frameDuration = 1f / currentAnimation.FPS;
                    spriteRenderer.Sprite = currentAnimation.Sprites[0]; //Only one animation in the spritesheet
                    MaxFrames = spriteRenderer.Sprite.Width / currentAnimation.FrameDimensions;
                }
            }
            catch (Exception)
            {
                throw new Exception($"Cant find the animation called {animationName} in Animations Dict");
            }
        }


    }
}
