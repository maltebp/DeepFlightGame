using DeepFlight.gui;
using DeepFlight.rendering;
using DeepFlight.src.gui;
using DeepFlight.src.gui.debugoverlay;
using DeepFlight.user;
using DeepFlight.user;
using DeepFlight.utility.KeyboardController;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;

namespace DeepFlight.scenes {

    class LoginScene : Scene {

        private Camera ui = new Camera();
        private TextureView title;
        private TextView errorText;
        private LoadingTextView loader;
        private TextInput textinput_Username, textinput_Password;
        private SimpleMenuOption menuoption_LoginAsGuest;
        private MenuView menu;

        protected override void OnInitialize() {

            // Height of UI screen
            var height = ScreenController.BaseHeight;
            var width = ScreenController.BaseWidth;

            // Adjust camera y=0 is top of screen
            ui.Y = height / 2;

            BackgroundColor = Settings.COLOR_PRIMARY;

            title = new TextureView(ui, Textures.TITLE);
            title.Height = (int)(height / 6);
            title.Width = title.Height * (Textures.TITLE.Width / Textures.TITLE.Height);
            title.X = 0;
            title.Y = height * 0.20;
            AddChild(title);

            textinput_Username = new TextInput(ui, "Username", Font.PIXELLARI, 36, Color.White, 0, height * 0.45, (float)width * 0.30f);
            textinput_Username.MaxLength = 20;
            textinput_Username.Focused = true;
            AddChild(textinput_Username);

            textinput_Password = new TextInput(ui, "Password", Font.PIXELLARI, 36, Color.White, 0, height * 0.65, (float)width * 0.30f);
            textinput_Password.PasswordInput = true;
            textinput_Password.MaxLength = 20;
            AddChild(textinput_Password);

            menuoption_LoginAsGuest = new SimpleMenuOption(ui, "Play as guest", Font.PIXELLARI, 24, Color.White);
            menuoption_LoginAsGuest.Y = height * 0.85;

            // Create loading component
            loader = new LoadingTextView(ui, y: height / 2);
            loader.Text = "Authenticating";
            loader.Hidden = true;
            AddChild(loader);

            errorText = new TextView(ui, "", Font.DEFAULT, 24, Color.White, 0, height * 0.75);
            errorText.Hidden = true;
            AddChild(errorText);

            menu = new MenuView();
            menu.AddMenuOption(textinput_Username, AttemptLogin);
            menu.AddMenuOption(textinput_Password, AttemptLogin);
            menu.AddMenuOption(menuoption_LoginAsGuest, LoginAsGuest);
            AddChild(menu);

        }


        public void ActivateInput() {
            loader.Hidden = true;
            menu.Hidden = false;

            // Select first element in menu
            menu.FocusedOptionIndex = 0;
        }


        protected override bool OnKeyInput(KeyEventArgs e) {
            if (e.Action == KeyAction.PRESSED) {
                if (e.Key == Keys.Escape) {
                    RequestExit();
                    return true;
                }
            }
            return false;
        }

        //private void SetInputFocus(TextInput inputToFocus) {
        //    textinput_Username.Focused = inputToFocus == textinput_Username;
        //    textinput_Password.Focused = inputToFocus == textinput_Password;
        //}

        //private void SwitchInputFocus() {
        //    textinput_Username.Focused = !textinput_Username.Focused;
        //    textinput_Password.Focused = !textinput_Password.Focused;
        //}

        private void ShowError(string error) {
            errorText.Text = "Error: " + error;
            errorText.Hidden = false;
        }


        private void AttemptLogin() {
            if (textinput_Username.Text.Length == 0) {
                ShowError("Enter a username!");
            }
            else if (textinput_Password.Text.Length == 0) {
                ShowError("Enter a password!");
            }
            else {
                AuthenticateUser(textinput_Username.Text, textinput_Password.Text);
            }
        }   



        private void AuthenticateUser(string username, string password) {
            ShowLoadingScreen();
            loading = true;
        }


        // TODO: Remove this once authentication has been implemented
        private bool loading = false;
        private double loadTimer = 1.5;
        protected override void OnUpdate(double deltaTime) {
            if (loading) {
                loadTimer -= deltaTime;
                if (loadTimer < 0) {
                    RequestSceneSwitch(new MainMenuScene());
                }
            }
        }

        // Log user in as guest, and switch to main menu
        private void LoginAsGuest() {
            User.LocalUser.Guest = true;
            RequestSceneSwitch(new MainMenuScene());
        }

        private void ShowLoadingScreen() {
            errorText.Hidden = true;
            menu.Hidden = true;
            loader.Hidden = false;
        }
    }

}
