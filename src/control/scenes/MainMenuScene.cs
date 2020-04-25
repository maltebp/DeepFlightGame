using DeepFlight.gui;
using DeepFlight.rendering;
using DeepFlight.src.gui;
using DeepFlight.user;
using DeepFlight.utility.KeyboardController;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace DeepFlight.scenes {
    class MainMenuScene : Scene {

        private Camera ui = new Camera();
        private SimpleMenuView menu;


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
            User.ResetLocalUser();
            RequestSceneSwitch(new LoginScene());
        }

        // Opens up the game website in default browser
        private void GoToWebsite() {
            System.Diagnostics.Process.Start(Settings.WEBSITE_URL);
        }

    }
}
