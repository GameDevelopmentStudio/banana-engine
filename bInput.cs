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

        protected GamePadState oldPadState;
        public GamePadState currentPadState;
        
        protected KeyboardState oldKeyState;
        public KeyboardState currentKeyState;

        public bInput()
        {
            currentPadState = GamePad.GetState(PlayerIndex.One);
            oldPadState = currentPadState;

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
            oldPadState = currentPadState;
            currentPadState = GamePad.GetState(PlayerIndex.One);

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

        public bool pressed(Buttons btn)
        {
            return currentPadState.IsButtonDown(btn) && oldPadState.IsButtonUp(btn);
        }

        public bool check(Buttons btn)
        {
            return currentPadState.IsButtonDown(btn);
        }

        public bool released(Buttons btn)
        {
            return currentPadState.IsButtonUp(btn) && oldPadState.IsButtonDown(btn);
        }

        public bool pressed(string id)
        {
            List<Object> registeredKeys;
            keys.TryGetValue(id, out registeredKeys);
            if (registeredKeys == null) return false;

            bool pressed = false;

            foreach (Object btn in registeredKeys)
            {
                if (btn is Buttons)
                    pressed = currentPadState.IsButtonDown((Buttons)btn) && oldPadState.IsButtonUp((Buttons)btn);
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

        public bool check(string id)
        {
            List<Object> registeredKeys;
            keys.TryGetValue(id, out registeredKeys);
            if (registeredKeys == null) return false;

            bool pressed = false;
            foreach (Object btn in registeredKeys)
            {
                if (btn is Buttons)
                    pressed = currentPadState.IsButtonDown((Buttons)btn);
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

        public bool released(string id)
        {
            List<Object> registeredKeys;
            keys.TryGetValue(id, out registeredKeys);
            bool pressed = false;

            foreach (Object btn in registeredKeys)
            {
                if (btn is Buttons)
                    pressed = currentPadState.IsButtonUp((Buttons)btn) && currentPadState.IsButtonDown((Buttons)btn);
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

        public bool left()
        {
            return currentPadState.ThumbSticks.Left.X < -0.3 ||
                currentKeyState.IsKeyDown(Keys.Left);
        }

        public bool right()
        {
            return currentPadState.ThumbSticks.Left.X > 0.3 ||
                currentKeyState.IsKeyDown(Keys.Right);
        }

        public bool up()
        {
            return currentPadState.ThumbSticks.Left.Y > 0.3 ||
                currentKeyState.IsKeyDown(Keys.Up);
        }

        public bool down()
        {
            return currentPadState.ThumbSticks.Left.Y < -0.3 ||
                currentKeyState.IsKeyDown(Keys.Down);
        }
    }
}
