using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace bEngine
{
    public class bInput
    {
        protected Dictionary<string, List<Object>> keys;

        Dictionary<PlayerIndex, GamePadState> oldPadStates;
        Dictionary<PlayerIndex, GamePadState> currentPadStates;
        
        protected KeyboardState oldKeyState;
        public KeyboardState currentKeyState;

        protected float joystickDeadzone = 0.4f;
        public float getJoystickDeadzone() { return joystickDeadzone; }
        public void setJoystickDeadzone(float value) { joystickDeadzone = value; }

        public bInput()
        {
            // Get all pads 
            currentPadStates = new Dictionary<PlayerIndex, GamePadState>();
            oldPadStates = new Dictionary<PlayerIndex, GamePadState>();
            foreach (PlayerIndex idx in  Enum.GetValues(typeof(PlayerIndex)))
            {
                currentPadStates[idx] = GamePad.GetState(idx);
                oldPadStates[idx] = currentPadStates[idx];
            }

            currentKeyState = Keyboard.GetState();
            oldKeyState = currentKeyState;

            keys = new Dictionary<string, List<Object>>();
        }

        public void registerKey(string id, Object key)
        {
            List<Object> registeredKeys;
            keys.TryGetValue(id, out registeredKeys);
            if (registeredKeys == null)
            {
                registeredKeys = new List<Object>();
                
            }

            registeredKeys.Add(key);
            keys[id] = registeredKeys;
        }

        public void update()
        {
            foreach (PlayerIndex idx in Enum.GetValues(typeof(PlayerIndex)))
            {
                oldPadStates[idx] = currentPadStates[idx];
                currentPadStates[idx] = GamePad.GetState(idx);
            }

            oldKeyState = currentKeyState;
            currentKeyState = Keyboard.GetState();
        }

        public bool pressed(Keys key)
        {
            return currentKeyState.IsKeyDown(key) && oldKeyState.IsKeyUp(key);
        }

        public bool check(Keys key)
        {
            return currentKeyState.IsKeyDown(key);
        }

        public bool released(Keys key)
        {
            return currentKeyState.IsKeyUp(key) && oldKeyState.IsKeyDown(key);
        }

        public bool pressed(Buttons btn, PlayerIndex idx = PlayerIndex.One)
        {
            return currentPadStates[idx].IsButtonDown(btn) && oldPadStates[idx].IsButtonUp(btn);
        }

        public bool check(Buttons btn, PlayerIndex idx = PlayerIndex.One)
        {
            return currentPadStates[idx].IsButtonDown(btn);
        }

        public bool released(Buttons btn, PlayerIndex idx = PlayerIndex.One)
        {
            return currentPadStates[idx].IsButtonUp(btn) && oldPadStates[idx].IsButtonDown(btn);
        }

        public bool pressed(string id, PlayerIndex idx = PlayerIndex.One)
        {
            List<Object> registeredKeys;
            keys.TryGetValue(id, out registeredKeys);
            if (registeredKeys == null) return false;

            bool pressed = false;

            foreach (Object btn in registeredKeys)
            {
                if (btn is Buttons)
                    pressed = currentPadStates[idx].IsButtonDown((Buttons)btn) && oldPadStates[idx].IsButtonUp((Buttons)btn);
                else if (btn is Keys)
                    pressed = currentKeyState.IsKeyDown((Keys)btn) && oldKeyState.IsKeyUp((Keys)btn);
                else
                {
                    Console.WriteLine("Could not check if key " + id + " was pressed");
                    return false;
                }

                if (pressed)
                    return true;
            }
            return false;
        }

        public bool check(string id, PlayerIndex idx = PlayerIndex.One)
        {
            List<Object> registeredKeys;
            keys.TryGetValue(id, out registeredKeys);
            if (registeredKeys == null) return false;

            bool pressed = false;
            foreach (Object btn in registeredKeys)
            {
                if (btn is Buttons)
                    pressed = currentPadStates[idx].IsButtonDown((Buttons)btn);
                else if (btn is Keys)
                    pressed = currentKeyState.IsKeyDown((Keys)btn);
                else
                {
                    Console.WriteLine("Could not check if key " + id + " was down");
                    return false;
                }

                if (pressed)
                    return true;
            }
            return false;
        }

        public bool released(string id, PlayerIndex idx = PlayerIndex.One)
        {
            List<Object> registeredKeys;
            keys.TryGetValue(id, out registeredKeys);
            bool pressed = false;

            foreach (Object btn in registeredKeys)
            {
                if (btn is Buttons)
                    pressed = currentPadStates[idx].IsButtonUp((Buttons)btn) && currentPadStates[idx].IsButtonDown((Buttons)btn);
                else if (btn is Keys)
                    pressed = currentKeyState.IsKeyUp((Keys)btn) && currentKeyState.IsKeyDown((Keys)btn);
                else
                {
                    Console.WriteLine("Could not check if key " + id + " was released");
                    return false;
                }

                if (pressed)
                    return true;
            }
            return false;
        }

        public bool left(PlayerIndex idx = PlayerIndex.One)
        {
            return currentPadStates[idx].ThumbSticks.Left.X < -joystickDeadzone ||
                currentKeyState.IsKeyDown(Keys.Left);
        }

        public bool right(PlayerIndex idx = PlayerIndex.One)
        {
            return currentPadStates[idx].ThumbSticks.Left.X > joystickDeadzone ||
                currentKeyState.IsKeyDown(Keys.Right);
        }

        public bool up(PlayerIndex idx = PlayerIndex.One)
        {
            return currentPadStates[idx].ThumbSticks.Left.Y > joystickDeadzone ||
                currentKeyState.IsKeyDown(Keys.Up);
        }

        public bool down(PlayerIndex idx = PlayerIndex.One)
        {
            return currentPadStates[idx].ThumbSticks.Left.Y < -joystickDeadzone ||
                currentKeyState.IsKeyDown(Keys.Down);
        }

        public int mouseX
        {
            get { return Mouse.GetState().X; }
        }

        public int mouseY
        {
            get { return Mouse.GetState().Y; }
        }

        public Vector2 mousePosition
        {
            get { return new Vector2(Mouse.GetState().X, Mouse.GetState().Y); }
        }
    }
}
