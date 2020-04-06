using DeepFlight.gui;
using DeepFlight.rendering;
using DeepFlight.src.gui;
using DeepFlight.utility.KeyboardController;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;

class MainMenuScene : Scene {

    private Camera ui = new Camera();
    private DrawableTexture background = new DrawableTexture(Textures.SQUARE, 500, 500);
    private DrawableTexture title = new DrawableTexture(Textures.TITLE);

    private LoadingView loader;

    private TextView errorText;

    private TextInput input_Username, input_Password;


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

        input_Username = new TextInput(ui, "Username", Fonts.PIXELLARI, 36, Color.White, 0, height * 0.45, (float)width*0.30f);
        input_Username.MaxLength = 20;
        input_Username.Focused = true;
        AddChild(input_Username);

        input_Password = new TextInput(ui, "Password", Fonts.PIXELLARI, 36, Color.White, 0, height * 0.65, (float) width*0.30f);
        input_Password.PasswordInput = true;
        input_Password.MaxLength = 20;
        AddChild(input_Password);

        // Create loading component
        loader = new LoadingView(ui, 0, height / 2);
        loader.Hidden = true;
        AddChild(loader);

        errorText = new TextView(ui, "", Fonts.DEFAULT, 24, Color.White, 0, height * 0.80);
        errorText.Hidden = true;
        AddChild(errorText);

    }


    protected override bool OnKeyInput(KeyEventArgs e) {
        
        // "Scroll" through input fields
        if( e.Action == KeyAction.PRESSED) {
            Console.WriteLine("Input!");
            if (e.Key == Keys.Up || e.Key == Keys.Down || e.Key == Keys.Tab) {
                SwitchInputFocus();
                return true;
            }
            if (e.Key == Keys.Enter) {
                if( input_Username.Text.Length == 0) {
                    SetInputFocus(input_Username);
                    ShowError("Enter a username!");
                }
                else if( input_Password.Text.Length == 0) {
                    SetInputFocus(input_Password);
                    ShowError("Enter a password!");
                }
                else {
                    Load();
                }
                return true;
            }
        }

        return false;
    }

    private void SetInputFocus(TextInput inputToFocus) {
        input_Username.Focused = inputToFocus == input_Username;
        input_Password.Focused = inputToFocus == input_Password;
    }

    private void SwitchInputFocus() {
        input_Username.Focused = !input_Username.Focused;
        input_Password.Focused = !input_Password.Focused;
    }

    private void ShowError(string error) {
        errorText.Text = "Error: " + error;
        errorText.Hidden = false;
    }
    
    private void Load() {
        errorText.Hidden = true;
        input_Username.Hidden = true;
        input_Password.Hidden = true;
        loader.Hidden = false;
    }


    protected override void OnDraw(Renderer renderer) {
        renderer.Draw(ui, background);
        renderer.Draw(ui, title);
    }
}