using DeepFlight.rendering;
using DeepFlight.src.gui;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeepFlight.src.scenes {
    class MainMenuScene : Scene {


        private Camera ui = new Camera();
        private TextureView background;
        private SimpleMenuView menu;
        private TextView tempText;

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

            menu = new SimpleMenuView(ui, Fonts.DEFAULT, 24, Color.White, 25);
            menu.AddOption("Option 1", () => Console.WriteLine("Option 1 selected!"));
            menu.AddOption("Option 2", () => Console.WriteLine("Option 2 selected!"));
            menu.AddOption("Option 3", () => Console.WriteLine("Option 3 selected!"));
            menu.AddOption("Option 4", () => Console.WriteLine("Option 4 selected!"));
            AddChild(menu);

            menu.Focused = true;

            
        }

    }
}
