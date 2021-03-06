﻿using DeepFlight.gui;
using DeepFlight.rendering;
using DeepFlight.src.gui;
using DeepFlight.user;
using DeepFlight.utility.KeyboardController;
using DeepFlight.view.gui;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace DeepFlight.scenes {


    /// <summary>
    /// The main menu of the game
    /// </summary>
    class MainMenuScene : Scene {

        private Camera ui = new Camera();
        private SimpleMenuView menu;
        private TextureView title;
        private BorderView border_User;
        private TextView text_PlayingAs;
        private TextView text_UserName;
        private TextView text_Guest;
        private TextView text_Rank;
        private TextView text_Version;


        protected override void OnInitialize() {

            // Height of UI screen
            float height = (float) ScreenController.BaseHeight;
            float width = (float) ScreenController.BaseWidth;

            // Adjust camera y=0 is top of screen
            ui.Y = height / 2;
            float menuX = -width * 0.25f;

            BackgroundColor = Settings.COLOR_PRIMARY;
            BackgroundTexture = Textures.BACKGROUND;

            // Title
            title = new TextureView(ui, Textures.TITLE);
            title.Height = (int)(height * 0.45);
            title.Width = title.Height * (Textures.TITLE.Width / (float)Textures.TITLE.Height);
            title.X = width * 0.22;
            title.Y = height * 0.45;
            AddChild(title);

            // Version text
            text_Version = new TextView(ui, "Version: " + Settings.VERSION, color: new Color(Color.White, 0.50f), size: 15, x: width * 0.49, y: height * 0.99);
            text_Version.HOrigin = HorizontalOrigin.RIGHT;
            text_Version.VOrigin = VerticalOrigin.BOTTOM;
            AddChild(text_Version);

            // Playing as box and text
            border_User = new BorderView(ui, borderWidth: 5f, x: menuX, y: height* 0.20, width: width * 0.27f, height: height * 0.23f);
            border_User.BackgroundColor = new Color(Color.Black, 0.25f);
            text_PlayingAs = new TextView(ui, "Playing as", size: 24, x: menuX, y:  height * 0.13);
            AddChildren(text_PlayingAs, border_User);

            // Add player name and rank to "Playing as"-box
            if( User.LocalUser.Guest) {
                text_Guest = new TextView(ui, "Guest", size: 34, x: menuX, y: height * 0.23);
                AddChild(text_Guest);
            }
            else {
                text_UserName = new TextView(ui, User.LocalUser.Username, size: 34, x: menuX, y: height * 0.20);
                text_Rank = new TextView(ui, "Not ranked", size: 24, x: menuX, y: height * 0.27);
                if (User.LocalUser.UniversalRank != 0) text_Rank.Text = "Rank #" + User.LocalUser.UniversalRank; 
                AddChildren(text_UserName, text_Rank);
            }

            // Setup Menu 
            menu = new SimpleMenuView(ui, Font.DEFAULT, 24, Color.White, 24);
            menu.Y = height * 0.40;
            menu.X = menuX;

            menu.AddSimpleOption("Online Tracks", () => RequestSceneSwitch(new OnlineTracksScene()));
            menu.AddSimpleOption("Offline Tracks", () => RequestSceneSwitch(new OfflineTracksScene()));
            menu.AddSimpleOption("Rankings", () => RequestSceneSwitch(new RankingsScene()));
            menu.AddSimpleOption("Website", () => GoToWebsite() );
            if( User.LocalUser.Guest )
                menu.AddSimpleOption("Login", () => ToLoginScene() );
            else
                menu.AddSimpleOption("Logout", () => ToLoginScene() );

            menu.AddSimpleOption("Exit", () => RequestExit()) ;
            AddChild(menu);

            menu.Focused = true;
        }

        // Exit to login scene if escape is presed
        protected override bool OnKeyInput(KeyEventArgs e) {
            if( e.Action == KeyAction.PRESSED) {
                if( e.Key == Keys.Escape) {
                    ToLoginScene();
                    return true;
                }
            }
            return false;
        }


        // Resets user and request scene switch to login scene
        private void ToLoginScene() {
            User.ResetLocalUser();
            RequestSceneSwitch(new LoginScene());
        }

        // Opens up the game website in default browser
        private void GoToWebsite() {
            System.Diagnostics.Process.Start(Settings.WEBSITE_URL);
        }

    }
}
