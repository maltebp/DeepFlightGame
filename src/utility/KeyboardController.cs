
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DeepFlight.utility.KeyboardController {

    static class KeyboardController {

        private static KeyboardState currentState;
        private static KeyboardState previousState;

        public delegate void KeyEventHandler(KeyEventArgs args);
        public static event KeyEventHandler KeyEvent;

        public delegate void CharEventHandler(CharEventArgs args);
        public static event CharEventHandler CharEvent;


        public static void Initialize(GameWindow window) {
            window.TextInput += OnTextInput;
        }

        /// <summary>
        /// Updates the state of the keyboard (used to check key presses). 
        /// </summary>
        public static void UpdateState() {
            previousState = currentState;
            currentState = Keyboard.GetState();

            if (previousState == null) return;

            var oldKeys = new LinkedList<Keys>(previousState.GetPressedKeys());

            // Check new key presses/held
            foreach( Keys key in currentState.GetPressedKeys()) {
                if( oldKeys.Contains(key) ) {
                    oldKeys.Remove(key);
                    OnKeyEvent(new KeyEventArgs(key, KeyAction.HELD));
                } else {
                    OnKeyEvent(new KeyEventArgs(key, KeyAction.PRESSED));
                }
            }

            // Old keys that are not pressed now 
            foreach(Keys key in oldKeys) {
                OnKeyEvent(new KeyEventArgs(key, KeyAction.RELEASED));
            }
        }

        public static void OnKeyEvent(KeyEventArgs args) {
            KeyEvent?.Invoke(args);
        }

        public static bool ShiftHeld() {
            return previousState != null && (IsHeld(Keys.LeftShift) || IsHeld(Keys.RightShift));
        }

        public static bool IsPressed(Keys key) {
            return currentState.IsKeyDown(key) && previousState.IsKeyUp(key);
        }

        public static bool IsHeld(Keys key) {
            return currentState.IsKeyDown(key) && previousState.IsKeyDown(key);
        }

        public static void OnTextInput(object sender, TextInputEventArgs args) {
            CharEvent?.Invoke(new CharEventArgs(args.Key, args.Character));
        }
    }

    public class KeyEventArgs : EventArgs {
        public Keys Key { get; }
        public KeyAction Action { get; }
        public bool Handled { get; set; } = false;
        public object KeyAction { get; internal set; }

        public KeyEventArgs(Keys key, KeyAction action) {
            Key = key;
            Action = action;
        }

        public override string ToString() {
            return string.Format("KeyEventArgs( key={0}, action{1}, handled={2} )",
               Key, Action.GetName(), Handled );
        }
    }


    // TODO: Implement this
    public class CharEventArgs : EventArgs {
        public Keys Key { get; }
        public char Character { get;  }
        public bool Handled { get; set; } = false;

        public CharEventArgs(Keys key, char character) {
            Key = key;
            Character = character;
        }

        public override string ToString() {
            return string.Format("CharEventArgs( key={0}, char={1} ({2}), handled={3} )",
               Key, Character, (int) Character, Handled);
        }
    }

    public enum KeyAction {
        PRESSED,
        RELEASED,
        HELD
    }

    public static class KeyActionMethods {
        /// <summary>
        /// Gets the name of the given VerticalOrigin, as it's
        /// written in the code (i.e. CENTER = "CENTER").
        /// </summary>
        public static string GetName(this KeyAction action) {
            return Enum.GetName(typeof(KeyAction), action);
        }
    }

}
