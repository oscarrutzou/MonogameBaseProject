using System.Collections.Generic;
using BaseProject.CompositPattern;
using Microsoft.Xna.Framework.Graphics;
using System.IO;

namespace BaseProject.GameManagement
{
    public enum AnimNames
    {
        TestWizardRightSheet,
        TestWizardRightIndividualFrames,
    }

    public static class GlobalAnimations
    {
        // Dictionary of all animations
        public static Dictionary<AnimNames, Animation> Animations { get; private set; }

        public static void LoadContent()
        {
            Animations = new Dictionary<AnimNames, Animation>();
            //Can upload sprite sheets, left to right
            LoadSpriteSheet(AnimNames.TestWizardRightSheet, "Test\\AnimationSheet\\wizardRight", 5f, 32);
            //wizardRight
            //How to use. Each animation should be called _0, then _1 and so on, on each texuture.
            //Remember the path should show everything and just delete the number. But keep the "_".
            LoadIndividualFramesAnimation(AnimNames.TestWizardRightIndividualFrames, "Test\\MultipleFilesAnim\\wizardRight", 5f, 8);
        }

        private static void LoadSpriteSheet(AnimNames animName, string path, float fps, int dem)
        {
            Texture2D[] sprite = new Texture2D[]{
                GameWorld.Instance.Content.Load<Texture2D>(path)
            };
            Animations.Add(animName, new Animation(animName, sprite, fps, dem));
        }

        private static void LoadIndividualFramesAnimation(AnimNames animationName, string path, float fps, int framesInAnim)
        {
            // Gets the aboslutePath to check what kind of animation it is.
            string contentRoot = Path.Combine(Directory.GetCurrentDirectory(), "Content");

            // You can use the absolutePath for loading, but we have chosen not to do that since it would be very messy when calling the Load methods
            string absolutePath = Path.Combine(contentRoot, path + " (1)" + ".xnb"); 

            if (File.Exists(absolutePath))
            {
                // Load the content
                Texture2D[] sprites = new Texture2D[framesInAnim];
                for (int i = 0; i < framesInAnim; i++)
                {
                    sprites[i] = GameWorld.Instance.Content.Load<Texture2D>(path + $" ({i + 1})");
                }
                Animations.Add(animationName, new Animation(animationName, sprites, fps));
            }
            else
            {
                throw new System.Exception("Cant find path in directory");
            }
        }
    }
}
