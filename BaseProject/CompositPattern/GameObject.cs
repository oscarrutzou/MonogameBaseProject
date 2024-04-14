using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BaseProject.CompositPattern
{
    // Make another one, where it just adds the gameobject to a Dictionary when a new type is created?
    // That is not one of the normal types like, animation, collision, spriterenderer and so on.
    public enum GameObjectTypes
    {
        Cell,
        Player,
        Enemy,
        Gui,
        Default, //Not set
    }

    public class GameObject
    {
        private Dictionary<Type, Component> components = new Dictionary<Type, Component>();
        public Transform Transform { get; private set; } = new Transform();

        public GameObjectTypes Type { get; set; } = GameObjectTypes.Default;


        public T AddComponent<T>() where T : Component
        {
            Type componentType = typeof(T);
            var constructor = componentType.GetConstructors().FirstOrDefault(c =>
            {
                var parameters = c.GetParameters();
                return parameters.Length == 1 && parameters[0].ParameterType == typeof(GameObject);
            });

            if (constructor != null)
            {
                T component = (T)Activator.CreateInstance(componentType, this);
                components[componentType] = component;
                return component;
            }
            else
            {
                throw new InvalidOperationException($"The class {componentType.Name} must have a constructor with one parameter of type GameObject.");
            }
        }

        public T AddComponent<T>(params object[] additionalParams) where T : Component
        {
            Type componentType = typeof(T);
            try
            {
                //Finds the constructor with the correct params
                object[] allParams = new object[1 + additionalParams.Length]; //Sets all params in an array
                allParams[0] = this;
                Array.Copy(additionalParams, 0, allParams, 1, additionalParams.Length);

                T component = (T)Activator.CreateInstance(componentType, allParams);
                components[componentType] = component;
                return component;
            }
            catch (Exception)
            {
                throw new Exception($"The class {componentType.Name} does not have a constructor that matches the provided parameters.");
            }
        }


        public T GetComponent<T>() where T : Component
        {
            Type componentType = typeof(T);
            if (components.TryGetValue(componentType, out Component component))
            {
                return (T)component;
            }

            return null;
        }

        public void Awake()
        {
            foreach (var component in components.Values)
            {
                component.Awake();
            }
        }

        public void Start()
        {
            foreach (var component in components.Values)
            {
                component.Start();
            }
        }

        public void Update(GameTime gameTime)
        {
            foreach (var component in components.Values)
            {
                component.Update(gameTime);
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (var component in components.Values)
            {
                component.Draw(spriteBatch);
            }
        }

        private Component AddComponentWithExistingValues(Component component)
        {
            components[component.GetType()] = component;
            return component;
        }

        public object Clone()
        {
            GameObject go = new GameObject();
            foreach (Component component in components.Values)
            {
                Component newComponent = go.AddComponentWithExistingValues(component.Clone() as Component);
                newComponent.SetNewGameObject(go);
            }
            return go;
        }

        public void SetLayerDepth(LAYERDEPTH layerDepth)
        {
            SpriteRenderer sr = GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                sr.SetLayerDepth(layerDepth);
                return;
            }

            throw new Exception($"You need to add a SpriteRenderer to set the layerdepth");
        }

        public void OnCollisionEnter(Collider collider)
        {
            foreach (var component in components.Values)
            {
                component.OnCollisionEnter(collider);
            }
        }
    }
}
