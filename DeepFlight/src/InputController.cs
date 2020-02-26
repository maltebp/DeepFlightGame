


using Microsoft.Xna.Framework.Input;

public static class InputController {

    private static KeyboardState newKeyboardState;
    private static KeyboardState oldKeyboardState;

    //private static MouseState newMouseState;
    //private static MouseState oldMouseState;

    public static void UpdateState() {
        oldKeyboardState = newKeyboardState;
        newKeyboardState = Keyboard.GetState();

        //newMouseState.

        //oldMouseState = newMouseState;

    }

    public static bool IsPressed( Keys key ) {
        return newKeyboardState.IsKeyDown(key) && oldKeyboardState.IsKeyUp(key);
    }

    public static bool IsHeld(Keys key) {
        return newKeyboardState.IsKeyDown(key);
    }



}

public static class InputExtension {
    public static bool IsPressed(this Keys key) {
        return InputController.IsPressed(key);
    }

    public static bool IsHeld (this Keys key) {
        return InputController.IsHeld(key);
    }
}