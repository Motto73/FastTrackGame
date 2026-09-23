using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace FastTrackGame
{
    //A more readable way to get input, allowing mapping to multiple keys
    static class Input
    {
        private static Dictionary<Key, KeyState> values = new Dictionary<Key, KeyState>
            {
                { Key.Up, KeyState.Nope },
                { Key.Right, KeyState.Nope },
                { Key.Down, KeyState.Nope },
                { Key.Left, KeyState.Nope },
                { Key.FireUp, KeyState.Nope },
                { Key.FireRight, KeyState.Nope },
                { Key.FireDown, KeyState.Nope },
                { Key.FireLeft, KeyState.Nope },
                { Key.Melee, KeyState.Nope },
                { Key.Fire, KeyState.Nope },
                { Key.Escape, KeyState.Nope },
                { Key.Space, KeyState.Nope },
                { Key.Debug, KeyState.Nope },
            };

        //Unfortunately I do not understand this part fully. Found online. Will read more on this before turning in!!
        [DllImport("user32.dll")]
        private static extern short GetAsyncKeyState(int vKey);


        public static void Update()
        {
            foreach(var kp in values)
            {
                var pressed = CheckPressed(kp.Key);
                if (pressed && (kp.Value == KeyState.Nope || kp.Value == KeyState.Released)) values[kp.Key] = KeyState.Pressed;
                else if (pressed && (kp.Value == KeyState.Pressed || kp.Value == KeyState.Held)) values[kp.Key] = KeyState.Held;
                else if (!pressed && (kp.Value == KeyState.Pressed || kp.Value == KeyState.Held)) values[kp.Key] = KeyState.Released;
                else values[kp.Key] = KeyState.Nope;
            }   
        }

        static bool CheckPressed(Key key)
        {
            foreach (var consoleKey in mappings[key])
            {
                if (GetAsyncKeyState(GetVirtualKeyCode(consoleKey)) < 0)
                    return true;
            }

            return false;
        }

        //Currently going from Key to ConsoleKey to VirtualKeyCode does not make much sense, but originally I had arrows and wasd map to the same movements
        private static Dictionary<Key, ConsoleKey[]> mappings = new()
        {
            { Key.Up,       [ConsoleKey.UpArrow] },
            { Key.Right,    [ConsoleKey.RightArrow] },
            { Key.Down,     [ConsoleKey.DownArrow] },
            { Key.Left,     [ConsoleKey.LeftArrow] },
            { Key.FireUp,   [ConsoleKey.W] },
            { Key.FireRight,[ConsoleKey.D] },
            { Key.FireDown, [ConsoleKey.S] },
            { Key.FireLeft, [ConsoleKey.A] },
            { Key.Melee,    [ConsoleKey.N] },
            { Key.Fire,     [ConsoleKey.M] },
            { Key.Escape,   [ConsoleKey.Escape] },
            { Key.Space,    [ConsoleKey.Spacebar] },
            { Key.Debug,    [ConsoleKey.P] }
        };

        //To use this hack to read keys, they need to convert from ConsoleKey to this horrible mess
        //I could have just written them directly in mappings but this is probably more readable
        //https://learn.microsoft.com/en-us/windows/win32/inputdev/virtual-key-codes
        private static int GetVirtualKeyCode(ConsoleKey key)
        {
            return key switch
            {
                ConsoleKey.W => 0x57,
                ConsoleKey.A => 0x41,
                ConsoleKey.S => 0x53,
                ConsoleKey.D => 0x44,

                ConsoleKey.UpArrow => 0x26,
                ConsoleKey.RightArrow => 0x27,
                ConsoleKey.DownArrow => 0x28,
                ConsoleKey.LeftArrow => 0x25,

                ConsoleKey.N => 0x4E,
                ConsoleKey.M => 0x4D,
                ConsoleKey.P => 0x50,

                ConsoleKey.Escape => 0x1B,
                ConsoleKey.Spacebar => 0x20,

                //No key somehow, throw exception
                _ => throw new ArgumentException($"No virtual key code for {key}!!!")
            };
        }

        //Public getters
        public static bool IsPressed(Key key)
        {
            return checkKey(key, KeyState.Pressed);
        }
        public static bool IsHeld(Key key)
        {
            return checkKey(key, KeyState.Held);
        }
        public static bool IsReleased(Key key)
        {
            return checkKey(key, KeyState.Released);
        }
        public static bool AnyMatching(Key[] keys, KeyState[] states)
        {
            foreach (var key in keys)
            {
                foreach (var state in states)
                    if (checkKey(key, state))
                        return true;
            }
            return false;
        }
        private static bool checkKey(Key key, KeyState state)
        {
            if (values.ContainsKey(key))
            {
                return values[key] == state;
            }
            else
                throw new ArgumentException($"No such key as {key.ToString()}!!");
        }


        //Map to WASD/Arrow keys and N+M, Debug = P
        public enum Key 
        {
            Up,
            Right,
            Down,
            Left,
            FireUp,
            FireRight,
            FireDown,
            FireLeft,
            Melee,
            Fire,
            Escape,
            Space,
            Debug
        }

        public enum KeyState
        {
            Nope,
            Pressed,
            Held,
            Released
        }
    }
}
