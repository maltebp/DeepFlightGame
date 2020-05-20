using DeepFlight.gui;
using DeepFlight.network;
using DeepFlight.network.exceptions;
using DeepFlight.rendering;
using DeepFlight.src.gui;
using DeepFlight.user;
using DeepFlight.utility;
using DeepFlight.utility.KeyboardController;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System.Diagnostics;

namespace DeepFlight.scenes {

    class LoginScene : Scene {

        private Camera ui = new Camera();
        private TextureView title;
        private TextView errorText;
        private TextView text_Login;
        private TextView text_Version;
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
            BackgroundTexture = Textures.BACKGROUND;


            title = new TextureView(ui, Textures.TITLE);
            title.Height = (int)(height * 0.45);
            title.Width = title.Height * (Textures.TITLE.Width / (float) Textures.TITLE.Height);
            title.X = width*0.22;
            title.Y = height * 0.45;
            AddChild(title);

            var menuX = -width * 0.25;

            text_Login = new TextView(ui, "Login", Font.DEFAULT, 38, Color.White, menuX, height * 0.20);
            AddChild(text_Login);

            textinput_Username = new TextInput(ui, "Username", Font.PIXELLARI, 36, Color.White, menuX, height * 0.40, (float)width * 0.30f);
            textinput_Username.MaxLength = 20;
            textinput_Username.Focused = true;
            AddChild(textinput_Username);

            textinput_Password = new TextInput(ui, "Password", Font.PIXELLARI, 36, Color.White, menuX, height * 0.60, (float)width * 0.30f);
            textinput_Password.PasswordInput = true;
            textinput_Password.Dictionairy = CharLists.DEFAULT + CharLists.SYMBOLS;
            textinput_Password.MaxLength = 20;
            AddChild(textinput_Password);

            menuoption_LoginAsGuest = new SimpleMenuOption(ui, "Play as guest", Font.PIXELLARI, 24, Color.White);
            menuoption_LoginAsGuest.Y = height * 0.75   ;
            menuoption_LoginAsGuest.X = menuX;

            // Create loading component
            loader = new LoadingTextView(ui, fontSize: 20, x: menuX, y: height* 0.5);
            loader.Text = "Authenticating";
            loader.Hidden = true;
            AddChild(loader);

            errorText = new TextView(ui, "", Font.DEFAULT, 20, Color.White, menuX, height * 0.70f);
            errorText.Hidden = true;
            AddChild(errorText);

            menu = new MenuView();
            menu.AddMenuOption(textinput_Username, AttemptLogin);
            menu.AddMenuOption(textinput_Password, AttemptLogin);
            menu.AddMenuOption(menuoption_LoginAsGuest, LoginAsGuest);
            AddChild(menu);

            text_Version = new TextView(ui, "Version: " + Settings.VERSION, color: new Color(Color.White, 0.50f), size: 15, x: width * 0.49, y: height * 0.99);
            text_Version.HOrigin = HorizontalOrigin.RIGHT;
            text_Version.VOrigin = VerticalOrigin.BOTTOM;
            AddChild(text_Version);

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


        /// <summary>
        /// Authenticates the credentials at the servers, and retrieve
        /// the user information
        /// </summary>
        private async void AuthenticateUser(string username, string password) {
            errorText.Hidden = true;
            menu.Hidden = true;
            loader.Hidden = false;

            try {
                string token;
                
                // Get authentication token
                try {
                    loader.Text = "Authenticating credentials";
                    var userApi = new UserAPIConnector();
                    token = await userApi.AuthenticateUser(username, password);
                }
                catch(AuthenticationException e) {
                    ShowError("Username and/or password incorrect");
                    return;
                }

                // Login to game server
                try {
                    loader.Text = "Fetching user information";
                    User.LocalUser = await new GameAPIConnector().GetUserPrivate(username, token);
                    RequestSceneSwitch(new MainMenuScene());
                }
                catch (AuthenticationException e) {
                    ShowError("Something went wrong on the server");
                    Trace.TraceError(string.Format("\nServer didn't accept the correct token '{0}' during login. Exception: {1}", token, e));
                }
            }
            catch(ConnectionException e) {
                ShowError("Cannot connect to server");
            }
            catch (ServerException e) {
                ShowError("Something went wrong on the server");
            }
        }


        // Log user in as guest, and switch to main menu
        private void LoginAsGuest() {
            var user = new User();
            user.Username = "Guest";
            user.Guest = true;
            User.LocalUser = user;
            RequestSceneSwitch(new MainMenuScene());
        }


        private void ShowError(string error) {
            errorText.Text = "Error: " + error;
            errorText.Hidden = false;
            menu.Hidden = false;
            loader.Hidden = true;
        }
    }

}
