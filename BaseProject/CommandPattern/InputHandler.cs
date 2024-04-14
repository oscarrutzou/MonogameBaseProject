using BaseProject.GameManagement;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using BaseProject.CommandPattern.Commands;

namespace BaseProject.CommandPattern
{
    public class InputHandler
    {
        #region Properties
        private static InputHandler instance;
        public static InputHandler Instance { get { return instance ??= instance = new InputHandler(); } }


        private Dictionary<Keys, ICommand> keybindsUpdate = new Dictionary<Keys, ICommand>();
        private Dictionary<Keys, ICommand> keybindsButtonDown = new Dictionary<Keys, ICommand>();
        private Dictionary<ButtonState, ICommand> mouseButtonUpdateCommands = new Dictionary<ButtonState, ICommand>();
        private Dictionary<ButtonState, ICommand> mouseButtonDownCommands = new Dictionary<ButtonState, ICommand>();

        public Vector2 mouseInWorld, mouseOnUI;
        public bool mouseOutOfBounds;

        #endregion

        private InputHandler()
        {
            AddUpdateCommand(Keys.Escape, new QuitCommand());
        }

        #region Command
        public void AddUpdateCommand(Keys inputKey, ICommand command)
        {
            keybindsUpdate.Add(inputKey, command);
        }

        public void AddButtonDownCommand(Keys inputKey, ICommand command)
        {
            keybindsButtonDown.Add(inputKey, command);
        }

        public void AddUpdateCommand(ButtonState inputButton, ICommand command)
        {
            mouseButtonUpdateCommands.Add(inputButton, command);
        }

        public void AddButtonDownCommand(ButtonState inputButton, ICommand command)
        {
            mouseButtonDownCommands.Add(inputButton, command);
        }

        private KeyboardState previousKeyState;
        private MouseState previousMouseState;
        public void Update()
        {
            KeyboardState keyState = Keyboard.GetState();
            MouseState mouseState = Mouse.GetState();

            mouseInWorld = GetMousePositionInWorld(mouseState);
            mouseOnUI = GetMousePositionOnUI(mouseState);

            UpdateKeyAndCommands(keyState);
            UpdateMouseCommands(mouseState);

            previousKeyState = keyState;
            previousMouseState = mouseState;
        }

        private void UpdateKeyAndCommands(KeyboardState keyState)
        {
            foreach (var pressedKey in keyState.GetPressedKeys())
            {
                if (keybindsUpdate.TryGetValue(pressedKey, out ICommand cmd))
                {
                    cmd.Execute();
                }
                if (!previousKeyState.IsKeyDown(pressedKey) && keyState.IsKeyDown(pressedKey))
                {
                    if (keybindsButtonDown.TryGetValue(pressedKey, out ICommand cmdBd))
                    {
                        cmdBd.Execute();

                    }
                }
            }
        }

        private void UpdateMouseCommands(MouseState mouseState)
        {
            // Check the left mouse button
            if (mouseState.LeftButton == ButtonState.Pressed && mouseButtonUpdateCommands.TryGetValue(ButtonState.Pressed, out ICommand cmd))
            {
                cmd.Execute();
            }

            if (previousMouseState.LeftButton == ButtonState.Released && mouseState.LeftButton == ButtonState.Pressed && mouseButtonDownCommands.TryGetValue(ButtonState.Pressed, out ICommand cmdBd))
            {
                cmdBd.Execute();
            }

            // Repeat similar checks for other button clicks.

            previousMouseState = mouseState;
        }

        #endregion

        private Vector2 GetMousePositionInWorld(MouseState mouseState)
        {
            Vector2 pos = new Vector2(mouseState.X, mouseState.Y);
            Matrix invMatrix = Matrix.Invert(GameWorld.Instance.WorldCam.GetMatrix());
            return Vector2.Transform(pos, invMatrix);
        }

        private Vector2 GetMousePositionOnUI(MouseState mouseState)
        {
            Vector2 pos = new Vector2(mouseState.X, mouseState.Y);
            Matrix invMatrix = Matrix.Invert(GameWorld.Instance.UiCam.GetMatrix());
            Vector2 returnValue = Vector2.Transform(pos, invMatrix);
            mouseOutOfBounds = (returnValue.X < 0 || returnValue.Y < 0 || returnValue.X > GameWorld.Instance.GfxManager.PreferredBackBufferWidth || returnValue.Y > GameWorld.Instance.GfxManager.PreferredBackBufferHeight);
            return returnValue;
        }

        public bool IsMouseOver(Rectangle collisionBox) => collisionBox.Contains(mouseInWorld);

    }
}
