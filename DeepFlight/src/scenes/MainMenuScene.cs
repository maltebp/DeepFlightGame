using DeepFlight.rendering;
using DeepFlight.src.gui;
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

        protected override void OnInitialize() {

            // Height of UI screen
            float height = (float) ScreenController.BaseHeight;
            float width = (float) ScreenController.BaseWidth;

            // Adjust camera y=0 is top of screen
            ui.Y = height / 2;

            // Create background
            background = new TextureView(ui, Textures.SQUARE);
            background.Col = Settings.COLOR_PRIMARY;
            background.Height = height;
            background.Width = width;
            background.VOrigin = VerticalOrigin.TOP;
            AddChild(background);

            menu = new SimpleMenuView(ui, Fonts.DEFAULT, 34, Color.White, 35);
            menu.Y = height * 0.20;

            menu.AddOption("Play Track", () => RequestSceneSwitch(scene: new TrackSelectionScene()));
            menu.AddOption("Ratings", () => Console.WriteLine("Ratings not implemented"));
            menu.AddOption("Settings", () => Console.WriteLine("Settings!"));
            menu.AddOption("Logout", () => Logout() );
            AddChild(menu);

            menu.Focused = true;
        }

        protected override bool OnKeyInput(KeyEventArgs e) {
            if( e.Action == KeyAction.PRESSED) {
                if( e.Key == Keys.Escape) {
                    Logout();
                    return true;
                }
            }
            return false;
        }


        private void Logout() {
            RequestSceneSwitch(new LoginScene());
        }

    }
}
