using DeepFlight.gui;
using DeepFlight.rendering;
using DeepFlight.src.gui;
using DeepFlight.src.user;
using DeepFlight.utility.KeyboardController;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeepFlight.scenes {
    class MainMenuScene : Scene {


        private Camera ui = new Camera();
        private TextureView background;
        private SimpleMenuView menu;

        private LoadingTextView loader;

        protected override void OnInitialize() {

            // Height of UI screen
            float height = (float) ScreenController.BaseHeight;
            float width = (float) ScreenController.BaseWidth;

            // Adjust camera y=0 is top of screen
            ui.Y = height / 2;
            BackgroundColor = Settings.COLOR_PRIMARY;

            // Setup Menu
            menu = new SimpleMenuView(ui, Font.DEFAULT, 34, Color.White, 35);
            menu.Y = height * 0.20;

            menu.AddSimpleOption("Play Track", () => RequestSceneSwitch(new OnlineTracksScene()));
            menu.AddSimpleOption("Rankings", () => RequestSceneSwitch(new RankingsScene()));
            menu.AddSimpleOption("Offline Tracks", () => RequestSceneSwitch(new OfflineTracksScene()));
            menu.AddSimpleOption("Website", () => GoToWebsite() );
            if( UserController.LoggedInAsGuest )
                menu.AddSimpleOption("Login", () => ToLoginScene() );
            else
                menu.AddSimpleOption("Logout", () => ToLoginScene() );

            menu.AddSimpleOption("Exit", () => RequestExit()) ;
            AddChild(menu);

            menu.Focused = true;
        }

        protected override bool OnKeyInput(KeyEventArgs e) {
            if( e.Action == KeyAction.PRESSED) {
                if( e.Key == Keys.Escape) {
                    ToLoginScene();
                    return true;
                }
            }
            return false;
        }


        private void ToLoginScene() {
            UserController.Logout();
            RequestSceneSwitch(new LoginScene());
        }

        // Opens up the game website in default browser
        private void GoToWebsite() {
            System.Diagnostics.Process.Start(Settings.WEBSITE_URL);
        }

    }
}
