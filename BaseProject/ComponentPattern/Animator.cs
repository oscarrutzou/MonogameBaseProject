﻿using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System;
using BaseProject.GameManagement;

namespace BaseProject.CompositPattern
{
    internal class Animator : Component
    {
        #region Properties
        private SpriteRenderer spriteRenderer;
        private Dictionary<AnimNames, Animation> animations = new Dictionary<AnimNames, Animation>();
        public Animation CurrentAnimation { get; private set; }

        /// <summary>
        /// If its false, the animation will play once, and then stop
        /// </summary>
        public bool IsLooping { get; set; }
        private bool hasPlayedAnim;

        public int CurrentIndex { get; private set; }
        public int MaxFrames;
        private float timeElapsed;
        private float frameDuration;


        public Animator(GameObject gameObject) : base(gameObject)
        {
        }
        #endregion

        public override void Start()
        {
            spriteRenderer = GameObject.GetComponent<SpriteRenderer>();
            if (spriteRenderer == null)
                throw new Exception($"No spriteRenderer on gameObject, and therefore its not possible to Animate");
        }

        public override void Update(GameTime gameTime)
        {
            if (CurrentAnimation == null) return;

            //if (IsLooping && hasPlayedAnim) return; // Already have played the animation once, so it can stop.
            timeElapsed += GameWorld.DeltaTime;

            if (CurrentAnimation.UseSpriteSheet)
            {
                UpdateSpriteSheet();
            }
            else
            {
                UpdateIndividualFrames();
            }
        }


        private void UpdateIndividualFrames()
        {
            if (timeElapsed > frameDuration && !hasPlayedAnim)
            {
                //Set new frame
                timeElapsed = 0;
                CurrentIndex = (CurrentIndex + 1) % CurrentAnimation.Sprites.Length;

                if (CurrentIndex == 0)
                {
                    if (!IsLooping) hasPlayedAnim = true; // Stops looping after playing once

                    CurrentAnimation.OnAnimationDone?.Invoke();
                }
            }

            spriteRenderer.Sprite = CurrentAnimation.Sprites[CurrentIndex];
        }

        private void UpdateSpriteSheet()
        {
            if (timeElapsed > frameDuration && !hasPlayedAnim)
            {
                //Set new frame
                timeElapsed = 0;
                CurrentIndex = (CurrentIndex + 1) % MaxFrames; //So it turns to 0 when it goes over maxframes

                if (CurrentIndex == 0)
                {
                    if (!IsLooping) hasPlayedAnim = true; // Stops looping after playing once

                    CurrentAnimation.OnAnimationDone?.Invoke();
                }
            }

            spriteRenderer.SourceRectangle.X = CurrentIndex * CurrentAnimation.FrameDimensions; // Only works with animation thats horizontal
        }

        public void AddAnimation(Animation animation) => animations.Add(animation.Name, animation);

        /// <summary>
        /// <para>Updates params based on chosen Animation. Also resets the IsLopping to true</para>
        /// </summary>
        /// <param name="animationName"></param>
        /// <exception cref="Exception"></exception>
        public void PlayAnimation(AnimNames animationName)
        {
            try
            {
                CurrentAnimation = animations[animationName];
                spriteRenderer.UsingAnimation = true; // This gets set to false if you have played a Animation, then want to use a normal sprite again
                frameDuration = 1f / CurrentAnimation.FPS; //Sets how long each frame should be
                IsLooping = true; // Resets loop
                if (CurrentAnimation.UseSpriteSheet)
                {
                    spriteRenderer.SourceRectangle = CurrentAnimation.SourceRectangle; // Use a sourcerectangle to only show the specific part of the animation
                    spriteRenderer.Sprite = CurrentAnimation.Sprites[0]; //Only one animation in the spritesheet
                    MaxFrames = spriteRenderer.Sprite.Width / CurrentAnimation.FrameDimensions; // Only works with animation thats horizontal
                }
            }
            catch (Exception)
            {
                throw new Exception($"Cant find the animation called {animationName} in Animations Dict");
            }
        }


    }
}
