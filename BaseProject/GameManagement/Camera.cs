using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace BaseProject.GameManagement
{
    public class Camera
    {
        public Vector2 position;          // The camera's position in the game world.
        public Vector2 origin;
        private float zoom;                // The zoom level of the camera.
        private Matrix transformMatrix;    // A transformation matrix used for rendering.
        public bool moveable;

        public Camera(bool moveable)
        {
            position = Vector2.Zero;   // Initialize the camera's position at the origin.
            zoom = 1.0f;               // Initialize the camera's zoom level to 1.0
            origin = new Vector2(GameWorld.Instance.GfxManager.PreferredBackBufferWidth / 2, GameWorld.Instance.GfxManager.PreferredBackBufferHeight / 2);
            this.moveable = moveable;
        }

        public void Move(Vector2 delta)
        {
            // Update the camera's position by adding a delta vector.
            position += delta;
        }

        public Vector2 TopLeft
        {
            get { return position - new Vector2(GameWorld.Instance.GfxManager.PreferredBackBufferWidth / 2, GameWorld.Instance.GfxManager.PreferredBackBufferHeight / 2); }
        }
        public Vector2 TopCenter
        {
            get { return position - new Vector2(0, GameWorld.Instance.GfxManager.PreferredBackBufferHeight / 2); }
        }

        public Vector2 TopRight
        {
            get { return position + new Vector2(GameWorld.Instance.GfxManager.PreferredBackBufferWidth / 2, -GameWorld.Instance.GfxManager.PreferredBackBufferHeight / 2); }
        }

        public Vector2 LeftCenter
        {
            get { return position - new Vector2(GameWorld.Instance.GfxManager.PreferredBackBufferWidth / 2, 0); }
        }

        public Vector2 Center
        {
            get { return position; }
        }

        public Vector2 RightCenter
        {
            get { return position + new Vector2(GameWorld.Instance.GfxManager.PreferredBackBufferWidth / 2, 0); }
        }

        public Vector2 BottomLeft
        {
            get { return position + new Vector2(-GameWorld.Instance.GfxManager.PreferredBackBufferWidth / 2, GameWorld.Instance.GfxManager.PreferredBackBufferHeight / 2); }
        }

        public Vector2 BottomCenter
        {
            get { return position + new Vector2(0, GameWorld.Instance.GfxManager.PreferredBackBufferHeight / 2); }
        }

        public Vector2 BottomRight
        {
            get { return position + new Vector2(GameWorld.Instance.GfxManager.PreferredBackBufferWidth / 2, GameWorld.Instance.GfxManager.PreferredBackBufferHeight / 2); }
        }



        public Matrix GetMatrix()
        {
            // Create a transformation matrix that represents the camera's view.
            // This matrix is used to adjust rendering based on the camera's position and zoom level.

            // 1. Translate to the negative of the camera's position.
            Matrix translationMatrix = Matrix.CreateTranslation(new Vector3(-position.X, -position.Y, 0));

            // 2. Scale the view based on the camera's zoom level.
            Matrix scaleMatrix = Matrix.CreateScale(zoom);

            // 3. Translate the view to center it on the screen.
            // This assumes the camera view is centered within the game window.
            // The following lines center the view using the screen's dimensions.
            Matrix centerMatrix = Matrix.CreateTranslation(new Vector3(origin.X, origin.Y, 0));

            // Combine the matrices in the correct order to create the final transformation matrix.
            transformMatrix = translationMatrix * scaleMatrix * centerMatrix;

            return transformMatrix; // Return the transformation matrix for rendering.
        }
    }
}
