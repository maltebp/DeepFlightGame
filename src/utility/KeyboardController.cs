
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;


namespace DeepFlight.utility.KeyboardController {
    
    /// <summary>
    /// Class which handles Keyboard events by comparing keyboard states
    /// The keyboard state is updated by the Application in the update loop
    /// </summary>
    public static class KeyboardController {

        private static KeyboardState currentState;
        private static KeyboardState previousState;

        public delegate void KeyEventHandler(KeyEventArgs args);
        public static event KeyEventHandler KeyEvent;

        public delegate void CharEventHandler(CharEventArgs args);
        public static event CharEventHandler CharEvent;


        public static void Initialize(GameWindow window) {
            window.TextInput += SignalTextInputEvent;
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
                    SignalKeyEvent(new KeyEventArgs(key, KeyAction.HELD));
                } else {
                    SignalKeyEvent(new KeyEventArgs(key, KeyAction.PRESSED));
                }
            }

            // Old keys that are not pressed now 
            foreach(Keys key in oldKeys) {
                SignalKeyEvent(new KeyEventArgs(key, KeyAction.RELEASED));
            }
        }

        // Signals that a new key event has occured (any key has been pressed)
        public static void SignalKeyEvent(KeyEventArgs args) {
            KeyEvent?.Invoke(args);
        }

        // Signals character input events (i.e. differs between small and large characters)
        public static void SignalTextInputEvent(object sender, TextInputEventArgs args) {
            CharEvent?.Invoke(new CharEventArgs(args.Key, args.Character));
        }

        // Determines whether or not shift is held
        public static bool IsShiftHeld() {
            return previousState != null && (IsHeld(Keys.LeftShift) || IsHeld(Keys.RightShift));
        }

        // Determines whether or not CTRL is held down
        public static bool IsCtrlHeld() {
            return previousState != null && (IsHeld(Keys.LeftControl) || IsHeld(Keys.RightControl));
        }

        // Tests whether or not a certain key is pressed
        // A key is pressed if it was tabbed in the frame, and not in the previous
        public static bool IsPressed(Keys key) {
            return currentState.IsKeyDown(key) && previousState.IsKeyUp(key);
        }

        // Tests whether or not a certain key is held
        // A key is held if it was pressed this frame and the previous frame
        public static bool IsHeld(Keys key) {
            return currentState.IsKeyDown(key) && previousState.IsKeyDown(key);
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
