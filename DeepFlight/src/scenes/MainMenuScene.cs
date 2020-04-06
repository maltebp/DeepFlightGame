
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

    private DrawableText text_Username, text_Password;

    private int currentInput = 0;

    protected override void OnInitialize() {

        // Height of UI screen
        var height = ScreenController.BaseHeight;
        var width = ScreenController.BaseWidth;
        
        // Adjust camera y=0 is top of screen
        ui.Y =  height / 2;

        Console.WriteLine("Texture: {0}x{1}", Textures.TITLE.Width, Textures.TITLE.Height);
        title.Height = (int) (height / 6);
        title.Width = title.Height * (Textures.TITLE.Width/Textures.TITLE.Height);
        title.X = 0;
        title.Y = height * 0.15;

        background = new DrawableTexture(Textures.SQUARE);
        background.Col = Settings.COLOR_PRIMARY;
        background.Height = (int) height;
        background.Width = (int) ScreenController.BaseWidth;
        background.VOrigin = VerticalOrigin.TOP;

        inputs[0] = new TextInput(ui, Fonts.PIXELLARI, 36, Color.White, 0, height * 0.40, width*0.25, ScreenController.BaseHeight/30);
        inputs[1] = new TextInput(ui, Fonts.PIXELLARI, 36, Color.White, 0, height * 0.65, width*0.25, ScreenController.BaseHeight / 30);
        inputs[1].PasswordInput = true;
        inputs[0].Text = "hello there world!";
        AddChild(inputs[0]);
        AddChild(inputs[1]);

        text_Username = new DrawableText("Username", Fonts.PIXELLARI, 24, Color.White,
            inputs[0].X - inputs[0].Width / 2, inputs[0].Y + inputs[0].Height / 2);
        text_Username.VOrigin = VerticalOrigin.TOP;
        text_Username.HOrigin = HorizontalOrigin.LEFT;

        inputs[0].Focused = true;

        Console.WriteLine(title);

        Console.WriteLine("\n\nFont scale test:");
        var width24 = Fonts.ROBOTO_BOLD.GetFont(24).MeasureString("Hello there world!").X;
        var width48 = Fonts.ROBOTO_BOLD.GetFont(48).MeasureString("Hello there world!").X;

        Console.WriteLine("Width 24: " + width24);
        Console.WriteLine("Width 48: " + width48);
        Console.WriteLine("Width scale: " + (width48/width24));
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
        //renderer.Draw(ui, text_Username);

    }
}