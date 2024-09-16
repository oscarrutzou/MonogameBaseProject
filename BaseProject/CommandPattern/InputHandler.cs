﻿using BaseProject.CommandPattern.Commands;
using BaseProject.CompositPattern;
using BaseProject.GameManagement;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System.Linq;

namespace BaseProject.CommandPattern;

public enum ScrollWheelState
{
    Up, Down
}

public enum MouseCmdState
{
    Left, Right
}

// Oscar
public class InputHandler
{
    #region Properties

    private static InputHandler instance;
    public static InputHandler Instance
    { get { return instance ??= instance = new InputHandler(); } }

    public KeyboardState KeyState;
    public MouseState MouseState;
    public Vector2 MouseInWorld, MouseOnUI;
    public bool DebugMode;
    public GameObject MouseGo;

    private Dictionary<Keys, List<Command>> _keybindsUpdate = new();
    private Dictionary<Keys, List<Command>> _keybindsButtonDown = new();
    private Dictionary<MouseCmdState, List<Command>> _mouseButtonUpdateCommands = new();
    private Dictionary<MouseCmdState, List<Command>> _mouseButtonDownCommands = new();
    private Dictionary<ScrollWheelState, List<Command>> _scrollWheelCommands = new();

    private KeyboardState _previousKeyState;
    private MouseState _previousMouseState;

    private List<Command> _allCommands = new List<Command>();

    #endregion Properties

    private InputHandler()
    {
        SetBaseKeys();
    }

    private void SetBaseKeys()
    {
        AddMouseButtonDownCommand(MouseCmdState.Left, new CheckButtonCmd());
        AddKeyButtonDownCommand(Keys.Escape, new QuitCommand());
    }

    #region Command

    #region Add/Remove

    public void AddKeyUpdateCommand(Keys inputKey, Command command)
    {
        if (!_keybindsUpdate.ContainsKey(inputKey))
        {
            _keybindsUpdate[inputKey] = new List<Command>();
        }
        _keybindsUpdate[inputKey].Add(command);
    }

    public void AddKeyButtonDownCommand(Keys inputKey, Command command)
    {
        if (!_keybindsButtonDown.ContainsKey(inputKey))
        {
            _keybindsButtonDown[inputKey] = new List<Command>();
        }
        _keybindsButtonDown[inputKey].Add(command);
    }

    public void AddMouseUpdateCommand(MouseCmdState inputButton, Command command)
    {
        if (!_mouseButtonUpdateCommands.ContainsKey(inputButton))
        {
            _mouseButtonUpdateCommands[inputButton] = new List<Command>();
        }
        _mouseButtonUpdateCommands[inputButton].Add(command);
    }

    public void AddMouseButtonDownCommand(MouseCmdState inputButton, Command command)
    {
        if (!_mouseButtonDownCommands.ContainsKey(inputButton))
        {
            _mouseButtonDownCommands[inputButton] = new List<Command>();
        }
        _mouseButtonDownCommands[inputButton].Add(command);
    }

    public void AddScrollWheelCommand(ScrollWheelState scrollWheelState, Command command)
    {
        if (!_scrollWheelCommands.ContainsKey(scrollWheelState))
        {
            _scrollWheelCommands[scrollWheelState] = new List<Command>();
        }
        _scrollWheelCommands[scrollWheelState].Add(command);
    }

    public void RemoveKeyUpdateCommand(Keys inputKey, Command commandToDelete = null)
    {
        if (!_keybindsUpdate.ContainsKey(inputKey)) return;

        if (commandToDelete != null)
            _keybindsUpdate[inputKey].Remove(commandToDelete);
        else
            _keybindsUpdate[inputKey].Clear();
    }

    public void RemoveKeyButtonDownCommand(Keys inputKey, Command commandToDelete = null)
    {
        if (!_keybindsButtonDown.ContainsKey(inputKey)) return;

        if (commandToDelete != null)
            _keybindsButtonDown[inputKey].Remove(commandToDelete);
        else
            _keybindsButtonDown[inputKey].Clear();
    }

    public void RemoveMouseUpdateCommand(MouseCmdState inputButton, Command commandToDelete = null)
    {
        if (!_mouseButtonUpdateCommands.ContainsKey(inputButton)) return;

        if (commandToDelete != null)
            _mouseButtonUpdateCommands[inputButton].Remove(commandToDelete);
        else
            _mouseButtonUpdateCommands[inputButton].Clear();
    }

    public void RemoveMouseButtonDownCommand(MouseCmdState inputButton, Command commandToDelete = null)
    {
        if (!_mouseButtonDownCommands.ContainsKey(inputButton)) return;

        if (commandToDelete != null)
            _mouseButtonDownCommands[inputButton].Remove(commandToDelete);
        else
            _mouseButtonDownCommands[inputButton].Clear();
    }

    public void RemoveScrollWheelCommand(ScrollWheelState scrollWheelState, Command commandToDelete = null)
    {
        if (!_scrollWheelCommands.ContainsKey(scrollWheelState)) return;

        if (commandToDelete != null)
            _scrollWheelCommands[scrollWheelState].Remove(commandToDelete);
        else
            _scrollWheelCommands[scrollWheelState].Clear();
    }


    /// <summary>
    /// Base Commands are the ones in the InputHandler, in the SetBaseKeys() method.
    /// </summary>
    public void RemoveAllExeptBaseCommands()
    {
        _keybindsUpdate.Clear();
        _keybindsButtonDown.Clear();
        _mouseButtonUpdateCommands.Clear();
        _mouseButtonDownCommands.Clear();
        _scrollWheelCommands.Clear();

        _allCommands.Clear();
        firstUpdate = true;

        SetBaseKeys();
    }

    #endregion Add/Remove


    private void SetAllCommands()
    {
        _allCommands.AddRange(_keybindsButtonDown.Values.SelectMany(cmdList => cmdList));
        _allCommands.AddRange(_mouseButtonUpdateCommands.Values.SelectMany(cmdList => cmdList));
        _allCommands.AddRange(_mouseButtonDownCommands.Values.SelectMany(cmdList => cmdList));
        _allCommands.AddRange(_scrollWheelCommands.Values.SelectMany(cmdList => cmdList));
    }

    private bool firstUpdate = true;

    public void Update()
    {
        if (firstUpdate)
        {
            SetAllCommands();
            firstUpdate = false;
        }

        KeyState = Keyboard.GetState();
        MouseState = Mouse.GetState();

        MouseInWorld = GetMousePositionInWorld(MouseState);
        MouseOnUI = GetMousePositionOnUI(MouseState);

        if (!GameWorld.Instance.IsActive) return;

        UpdateAllCommands();

        UpdateKeyCommands(KeyState);
        UpdateMouseCommands(MouseState);

        _previousKeyState = KeyState;
        _previousMouseState = MouseState;
    }

    private void UpdateAllCommands()
    {
        // Updates each command
        foreach (var cmd in _allCommands)
        {
            cmd.Update();
        }
    }

    private void UpdateKeyCommands(KeyboardState keyState)
    {
        foreach (var pressedKey in keyState.GetPressedKeys())
        {
            if (_keybindsUpdate.TryGetValue(pressedKey, out List<Command> cmds) && cmds.Count > 0) // Commands that happen every update
            {
                foreach (var cmd in cmds)
                {
                    cmd.Execute();
                }
            }
            if (!_previousKeyState.IsKeyDown(pressedKey) && keyState.IsKeyDown(pressedKey)) // Commands that only happens once every time the button gets pressed
            {
                if (_keybindsButtonDown.TryGetValue(pressedKey, out List<Command> cmdsBd) && cmdsBd.Count > 0)
                {
                    foreach (var cmdBd in cmdsBd)
                    {
                        cmdBd.Execute();
                    }
                }
            }
        }
    }

    private void UpdateMouseCommands(MouseState mouseState)
    {
        // Left mouse button update commands
        if (mouseState.LeftButton == ButtonState.Pressed
            && _mouseButtonUpdateCommands.TryGetValue(MouseCmdState.Left, out List<Command> cmdsLeft) && cmdsLeft.Count > 0)
        {
            foreach (var cmdLeft in cmdsLeft)
            {
                cmdLeft.Execute();
            }
        }

        // Left mouse button down commands
        if (_previousMouseState.LeftButton == ButtonState.Released
            && mouseState.LeftButton == ButtonState.Pressed
            && _mouseButtonDownCommands.TryGetValue(MouseCmdState.Left, out List<Command> cmdsBdLeft) && cmdsBdLeft.Count > 0)
        {
            foreach (var cmdBdLeft in cmdsBdLeft)
            {
                cmdBdLeft.Execute();
            }
        }

        // Right mouse button update commands
        if (mouseState.RightButton == ButtonState.Pressed
            && _mouseButtonUpdateCommands.TryGetValue(MouseCmdState.Right, out List<Command> cmdsRight) && cmdsRight .Count > 0)
        {
            foreach (var cmdRight in cmdsRight)
            {
                cmdRight.Execute();
            }
        }

        // Right mouse button down commands
        if (_previousMouseState.RightButton == ButtonState.Released
            && mouseState.RightButton == ButtonState.Pressed
            && _mouseButtonDownCommands.TryGetValue(MouseCmdState.Right, out List<Command> cmdsBdRight) && cmdsBdRight.Count > 0)
        {
            foreach (var cmdBdRight in cmdsBdRight)
            {
                cmdBdRight.Execute();
            }
        }

        // Checks the Scroll wheel and gets the appropriately command
        if (_previousMouseState.ScrollWheelValue != mouseState.ScrollWheelValue
            && _scrollWheelCommands.TryGetValue(
                mouseState.ScrollWheelValue > _previousMouseState.ScrollWheelValue
                ? ScrollWheelState.Up : ScrollWheelState.Down, out List<Command> cmdsScroll) && cmdsScroll.Count > 0)
        {
            foreach (var cmdScroll in cmdsScroll)
            {
                cmdScroll.Execute();
            }
        }

        _previousMouseState = mouseState;
    }

    #endregion Command

    private Vector2 GetMousePositionInWorld(MouseState mouseState)
    {
        Vector2 pos = new Vector2(mouseState.X, mouseState.Y);
        Matrix invMatrix = Matrix.Invert(GameWorld.Instance.WorldCam.GetMatrix());
        return Vector2.Transform(pos, invMatrix);
    }

    private Vector2 GetMousePositionOnUI(MouseState mouseState)
    {
        Camera uiCam = GameWorld.Instance.UiCam;
        Vector2 pos = new Vector2(mouseState.X, mouseState.Y);
        Matrix invMatrix = Matrix.Invert(uiCam.GetMatrix());
        Vector2 returnValue = Vector2.Transform(pos, invMatrix);
        //MouseOutOfBounds = (returnValue.X < uiCam.TopLeft.X || returnValue.Y < uiCam.TopLeft.Y || returnValue.X > uiCam.BottomRight.X || returnValue.Y > uiCam.BottomRight.Y);
        return returnValue;
    }
}