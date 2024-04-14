using System.Collections.Generic;
using BaseProject.GameManagement;
using Microsoft.Xna.Framework;

namespace BaseProject.CompositPattern.Grid
{
    public class Grid : Component
    {
        public Vector2 startPostion { get; set; }

        public Dictionary<Point, GameObject> Cells { get; private set; } = new Dictionary<Point, GameObject>();
        public List<Point> TargetPoints { get; private set; } = new List<Point>(); //Target cell points
        private int width, height;

        public int mapW, mapH;

        private bool isCentered = true;

        private int demension => Cell.demension;

        public Grid(GameObject gameObject) : base(gameObject)
        {

        }


        public void GenerateGrid(Vector2 startPos, int width, int height)
        {
            #region Set Params
            this.width = width;
            this.height = height;

            if (isCentered)
            {
                startPos = new Vector2(
                    startPos.X - (width * demension * Cell.scaleSize.X / 2),
                    startPos.Y - (height * demension * Cell.scaleSize.Y / 2)
                );
            }

            this.startPostion = startPos;
            #endregion

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    Point point = new Point(x, y);
                    GameObject cellGo = new GameObject();
                    cellGo.AddComponent<Cell>(this, point);
                    cellGo.Type = GameObjectTypes.Cell;
                    SpriteRenderer sr = cellGo.AddComponent<SpriteRenderer>();
                    sr.SetLayerDepth(LAYERDEPTH.WorldBackground);
                    sr.SetSprite(TextureNames.Cell);

                    if ((x + y) % 2 == 0) sr.Color = new Color(30, 150, 20); // Set color so every second one is colored

                    Cells.Add(point, cellGo);
                    GameWorld.Instance.Instantiate(cellGo);
                }
            }
        }

        public GameObject GetCellGameObject(Vector2 pos)
        {
            if (pos.X < startPostion.X || pos.Y < startPostion.Y)
            {
                return null; // Position is negative, otherwise it will make a invisable tile in the debug, since it cast to int, then it gets rounded to 0 and results in row and column
            }

            int gridX = (int)((pos.X - startPostion.X) / (Cell.demension * Cell.scaleSize.X));
            int gridY = (int)((pos.Y - startPostion.Y) / (Cell.demension * Cell.scaleSize.Y));

            if (0 <= gridX && gridX < width && 0 <= gridY && gridY < height)
            {
                return Cells[new Point(gridX, gridY)];
            }

            return null; // Position is out of bounds
        }

        public Vector2 PosFromGridPos(Point point) => Cells[point].Transform.Position;

        public GameObject GetCellGameObjectFromPoint(Point point) => GetCellGameObject(PosFromGridPos(point));
        public Cell GetCellFromPoint(Point point) => GetCellGameObjectFromPoint(point).GetComponent<Cell>();
    }
}
