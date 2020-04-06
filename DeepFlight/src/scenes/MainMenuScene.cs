
using DeepFlight.gui;
using DeepFlight.rendering;
using DeepFlight.utility.KeyboardController;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;

class MainMenuScene : Scene {

    private Camera ui = new Camera();
    private DrawableTexture background = new DrawableTexture(Textures.SQUARE, 500, 500);
    private DrawableTexture title = new DrawableTexture(Textures.TITLE);


    private TextInput[] inputs = new TextInput[2];
    private int currentInput = 0;

    protected override void OnInitialize() {

        DrawableTexture ship = new Ship();
        Console.WriteLine(ship);

        Console.WriteLine("Texture: {0}x{1}", Textures.TITLE.Width, Textures.TITLE.Height);
        title.Height = (int) (ScreenController.BaseHeight / 6);
        title.Width = title.Height * (Textures.TITLE.Width/Textures.TITLE.Height);
        title.X = 0;
        title.Y = -ScreenController.BaseHeight/3;

        background = new DrawableTexture(Textures.SQUARE);
        background.Col = Settings.COLOR_PRIMARY;
        background.Height = (int) ScreenController.BaseHeight;
        background.Width = (int) ScreenController.BaseWidth;

        inputs[0] = new TextInput(ui, Fonts.ROBOTO_BOLD, 24, Color.White, 0, -ScreenController.BaseHeight / 5, ScreenController.BaseWidth/8, ScreenController.BaseHeight/30);
        inputs[1] = new TextInput(ui, Fonts.ROBOTO_BOLD, 24, Color.White, 0, -ScreenController.BaseHeight / 8, ScreenController.BaseWidth / 8, ScreenController.BaseHeight / 30);
        inputs[1].PasswordInput = true;
        inputs[0].Text = "iiiiiiiiiiiiiii";

        AddChild(inputs[0]);
        AddChild(inputs[1]);

        inputs[0].Focused = true;

        Console.WriteLine(title);
    }


    protected override bool OnKeyInput(KeyEventArgs e) {
        
        // "Scroll" through input fields
        if( e.Action == KeyAction.PRESSED) {
            if (e.Key == Keys.Up) {
                AdjustFocus(-1);
                return true;
            }
            if (e.Key == Keys.Down || e.Key == Keys.Tab) {
                AdjustFocus(1);
                return true;
            }
        }

        return false;
    }

    private void AdjustFocus(int i) {
        foreach(var input in inputs) {
            input.Focused = false;
        }
        currentInput = MathExtension.Mod(currentInput+1, inputs.Length);
        inputs[currentInput].Focused = true;
    }


    private int resolutionIndex = 0;
    protected override void OnDraw(Renderer renderer) {

        renderer.Draw(ui, background);
        renderer.Draw(ui, title);

    }
}