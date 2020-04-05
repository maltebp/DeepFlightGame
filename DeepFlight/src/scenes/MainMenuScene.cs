
using Deepflight.Globals;
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

    private TextInput usernameInput; 

    protected override void OnInitialize() {

        DrawableTexture ship = new Ship();
        Console.WriteLine(ship);

        Console.WriteLine("Texture: {0}x{1}", Textures.TITLE.Width, Textures.TITLE.Height);
        title.Height = (int) (ScreenManager.BaseHeight / 6);
        title.Width = title.Height * (Textures.TITLE.Width/Textures.TITLE.Height);
        title.X = 0;
        title.Y = -ScreenManager.BaseHeight/3;

        background = new DrawableTexture(Textures.SQUARE);
        background.Col = Globals.COLOR_PRIMARY;
        background.Height = (int) ScreenManager.BaseHeight;
        background.Width = (int) ScreenManager.BaseWidth;

        usernameInput = new TextInput(ui, Fonts.ARIAL, 24, Color.White, 0, -ScreenManager.BaseHeight / 5, ScreenManager.BaseWidth/8, ScreenManager.BaseHeight/30);

        AddChild(usernameInput);

        usernameInput.Focused = true;

        Console.WriteLine(title);
    }


    //public void OnKeyEvent(KeyEventArgs args) {
    //    if (args.Handled) return;

    //    if( args.Action == KeyAction.PRESSED) {
    //        if (usernameInput.KeyInput(args.Key)) {
    //            args.Handled = true;
    //            return;
    //        }
    //    }
    //}


    private int resolutionIndex = 0;
    protected override void OnDraw(Renderer renderer) {

        renderer.Draw(ui, background);
        renderer.Draw(ui, title);

    }
}