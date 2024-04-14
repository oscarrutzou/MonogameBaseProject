using Microsoft.Xna.Framework;

namespace BaseProject.CompositPattern.Grid
{
    public class Cell : Component
    {
        public static int demension = 16;
        public static Vector2 scaleSize = new Vector2(4, 4);
        public bool isValid = true;

        public int cost = 1;
        public int G;
        public int H;
        public int F => G + H;

        /// <summary>
        /// Parent is for the Astar, not the GameObject that is attached as "GameObject".
        /// </summary>
        public GameObject Parent { get; set; }


        public Cell(GameObject gameObject, Grid grid, Point point) : base(gameObject)
        {
            GameObject.Transform.GridPosition = point;
            gameObject.Transform.Position = grid.startPostion + new Vector2(point.X * demension * scaleSize.X + demension * scaleSize.X / 2, point.Y * demension * scaleSize.Y + demension * scaleSize.Y / 2);
            GameObject.Transform.Scale = scaleSize;
        }

        public void Reset()
        {
            Parent = null;
            G = 0;
            H = 0;
        }

    }
}
