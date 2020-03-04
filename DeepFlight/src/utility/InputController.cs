using Microsoft.Xna.Framework.Input;


/// <summary>
/// Handles the evaluation of whether keys are pressed or held.
/// </summary>
public static class InputController {

    private static KeyboardState newKeyboardState;
    private static KeyboardState oldKeyboardState;

    private static MouseState newMouseState;
    private static MouseState oldMouseState;

    /// <summary>
    /// Updates the state of the keyboard (used to check key presses). 
    /// </summary>
    public static void UpdateState() {
        oldKeyboardState = newKeyboardState;
        newKeyboardState = Keyboard.GetState();

        oldMouseState = newMouseState;
        newMouseState = Mouse.GetState();
    }

    /// <summary>
    /// Tests if the key was pressed in this update (meaning it was not pressed in previous frame)
    /// </summary>
    public static bool IsPressed( Keys key ) {
        return newKeyboardState.IsKeyDown(key) && oldKeyboardState.IsKeyUp(key);
    }

    /// <summary>
    /// Tests if the key has been being pressed (regardless of previous state being released).
    /// </summary>
    public static bool IsHeld(Keys key) {
        return newKeyboardState.IsKeyDown(key);
    }

    /// <summary>
    /// Returns how much the mousewheel value has changed since last update
    /// </summary>
    public static int MouseWheelDiff() {
        return newMouseState.ScrollWheelValue - oldMouseState.ScrollWheelValue;
    }

}

// Extension methods for Keys, to easily check its "state"
public static class InputExtension {

    /// <summary>
    /// Tests if the key was pressed in this update (meaning it was not pressed in previous frame)
    /// </summary>
    public static bool IsPressed(this Keys key) {
        return InputController.IsPressed(key);
    }

    /// <summary>
    /// Tests if the key has been being pressed (regardless of previous state being released).
    /// </summary>
    public static bool IsHeld (this Keys key) {
        return InputController.IsHeld(key);
    }    
}
