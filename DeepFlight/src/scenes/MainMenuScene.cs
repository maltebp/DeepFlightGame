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

        inputs[0] = new TextInput(ui, "Username", Fonts.PIXELLARI, 36, Color.White, 0, height * 0.45, (float)width*0.30f);
        inputs[0].MaxLength = 20;

        inputs[1] = new TextInput(ui, "Password", Fonts.PIXELLARI, 36, Color.White, 0, height * 0.65, (float) width*0.30f);
        inputs[1].PasswordInput = true;
        inputs[1].MaxLength = 20;

        AddChild(inputs[0]);
        AddChild(inputs[1]);



        //text_Username = new DrawableText("Username", Fonts.PIXELLARI, 20, Color.White,
        //    inputs[0].X - inputs[0].Width / 2, inputs[0].Y + height*0.03);
        //text_Username.VOrigin = VerticalOrigin.TOP;
        //text_Username.HOrigin = HorizontalOrigin.LEFT;

        //text_Password = new DrawableText("Password", Fonts.PIXELLARI, 20, Color.White,
        //    inputs[1].X - inputs[1].Width / 2, inputs[1].Y + height * 0.03);
        //text_Password.VOrigin = VerticalOrigin.TOP;
        //text_Password.HOrigin = HorizontalOrigin.LEFT;

        inputs[0].Focused = true;
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