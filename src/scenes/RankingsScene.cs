using DeepFlight.rendering;
using DeepFlight.utility.KeyboardController;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeepFlight.scenes {
    class RankingsScene : Scene {

        private Camera camera_UI = new Camera();

        private TextView sceneTitle;

        private TextView text_Error;

        protected override void OnInitialize() {

            // Height of UI screen
            float height = (float) ScreenController.BaseHeight;
            float width = (float) ScreenController.BaseWidth;

            // Adjust camera y=0 is top of screen
            camera_UI.Y = height / 2;

            // General background of Scene
            BackgroundColor = Settings.COLOR_PRIMARY;

            sceneTitle = new TextView(camera_UI, "Rankings", Font.DEFAULT, 42, Color.White, 0, height * 0.20);
            AddChild(sceneTitle);

            // Text for displaying errors on center of screen
            text_Error = new TextView(camera_UI, "", Font.DEFAULT, 32, Color.White, 0, height * 0.5);
            text_Error.Hidden = true;
            AddChild(text_Error);

            // TODO: Remove this once the scene is implemented
            DisplayError("This scene is not implemented yet! :( Come back later...");
        }


        protected override bool OnKeyInput(KeyEventArgs e) {
            if( e.Action == KeyAction.PRESSED) {
                if( e.Key == Keys.Escape) {
                    RequestSceneSwitch(new MainMenuScene());
                    return true;
                }
            }

            return false;
        }

        private void DisplayError(string message) {
            text_Error.Text = "Error: " + message;
            text_Error.Hidden = false;
        }

    }
}
