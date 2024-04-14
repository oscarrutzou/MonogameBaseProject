using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using BaseProject.CompositPattern;
using BaseProject.CompositPattern.Grid;

namespace BaseProject.GameManagement
{
    public enum ScenesNames
    {
        //MainMenu,
        //LoadingScreen,
        GameScene,
        OscarTestScene,
        StefanTestScene,
        ErikTestScene,
        AsserTestScene,
        //EndMenu,
    }


    public abstract class Scene
    {
        // We have a data stored on each scene, to make it easy to add and remove gameObjects
        public bool hasFadeOut;
        public bool isPaused;

        private List<GameObject> newGameObjects = new List<GameObject>();
        private List<GameObject> destoroyedGameObjects = new List<GameObject>();

        public abstract void Initialize();

        /// <summary>
        /// The base update on the scene handles all the gameobjects and calls Update on them all. 
        /// </summary>
        public virtual void Update(GameTime gameTime)
        {
            CleanUp();

            foreach (GameObject gameObject in SceneData.gameObjects)
            {
                gameObject.Update(gameTime);
            }

            CheckCollision();
        }


        public void Instantiate(GameObject gameObject) => newGameObjects.Add(gameObject);

        public void Destroy(GameObject go) => destoroyedGameObjects.Add(go);

        private void CleanUp()
        {
            if (newGameObjects.Count == 0 && destoroyedGameObjects.Count == 0) return; //Shouldnt run since there is no new changes

            for (int i = 0; i < newGameObjects.Count; i++)
            {
                SceneData.gameObjects.Add(newGameObjects[i]);
                AddToCategory(newGameObjects[i]);
                newGameObjects[i].Awake();
                newGameObjects[i].Start();
            }
            for (int i = 0; i < destoroyedGameObjects.Count; i++)
            {
                SceneData.gameObjects.Remove(destoroyedGameObjects[i]);
                RemoveFromCategory(destoroyedGameObjects[i]);
            }

            newGameObjects.Clear();
            destoroyedGameObjects.Clear();
        }

        private void AddToCategory(GameObject gameObject)
        {
            switch (gameObject.Type)
            {
                case GameObjectTypes.Cell:
                    SceneData.cells.Add(gameObject.GetComponent<Cell>());
                    break;
                case GameObjectTypes.Gui:
                    SceneData.guis.Add(gameObject);
                    break;
                default:
                    SceneData.defaults.Add(gameObject);
                    break;
            }
        }

        private void RemoveFromCategory(GameObject gameObject)
        {
            switch (gameObject.Type)
            {
                case GameObjectTypes.Cell:
                    SceneData.cells.Remove(gameObject.GetComponent<Cell>());
                    break;
                case GameObjectTypes.Gui:
                    SceneData.guis.Remove(gameObject);
                    break;
                default:
                    SceneData.defaults.Remove(gameObject);
                    break;
            }
        }

        public virtual void DrawInWorld(SpriteBatch spriteBatch)
        {
            //DrawSceenColor();

            //// Draw all GameObjects that is not Gui in the active scene.
            foreach (GameObject gameObject in SceneData.gameObjects)
            {
                if (gameObject.Type != GameObjectTypes.Gui)
                {
                    gameObject.Draw(spriteBatch);
                }
            }
        }

        public virtual void DrawOnScreen(SpriteBatch spriteBatch)
        {
            // Draw all Gui GameObjects in the active scene.
            foreach (GameObject guiGameObject in SceneData.guis)
            {
                guiGameObject.Draw(spriteBatch);
            }
        }

        public void CheckCollision()
        {
            foreach (GameObject go1 in SceneData.gameObjects)
            {
                foreach (GameObject go2 in SceneData.gameObjects)
                {
                    if (go1 == go2) continue;
                    //Dosent check between enemies
                    //Enemy enemy1 = go1.GetComponent<Enemy>();
                    //Enemy enemy2 = go2.GetComponent<Enemy>();
                    //if (enemy1 != null && enemy2 != null) continue; //Shouldnt make collisions between 2 enemies.

                    Collider col1 = go1.GetComponent<Collider>();
                    Collider col2 = go2.GetComponent<Collider>();

                    //Check base collisionbox
                    if (col1 != null && col2 != null && col1.CollisionBox.Intersects(col2.CollisionBox))
                    {
                        go1.OnCollisionEnter(col2);
                        go2.OnCollisionEnter(col1);
                    }
                }
            }
        }
    }
}
