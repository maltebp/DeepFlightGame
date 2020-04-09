﻿using DeepFlight.rendering;
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
    class TrackCompleteScene : Scene {


        private Camera uiCamera = new Camera();
        private TextureView background;
        private SimpleMenuView menu;
        private TextView text_TrackComplete;
        private TextView text_TimeText;
        private TextView text_TimeValue;

        private Track track;
        private TimeSpan time;

        public TrackCompleteScene(Track track, TimeSpan time) {
            this.track = track;
            this.time = time;
        }

        protected override void OnInitialize() {

            // Height of UI screen
            float height = (float)ScreenController.BaseHeight;
            float width = (float)ScreenController.BaseWidth;

            // Adjust camera y=0 is top of screen
            uiCamera.Y = height / 2;

            // Create background
            background = new TextureView(uiCamera, Textures.SQUARE);
            background.Col = Settings.COLOR_PRIMARY;
            background.Height = height;
            background.Width = width;
            background.VOrigin = VerticalOrigin.TOP;
            AddChild(background);

            text_TrackComplete = new TextView(uiCamera, "Track Complete!", Fonts.PIXELLARI, 40, Color.White, 0, height*0.15);
            AddChild(text_TrackComplete);

            text_TimeText = new TextView(uiCamera, "Your time:", Fonts.PIXELLARI, 24, Color.White, 0, height*0.30 );
            AddChild(text_TimeText);

            var timeString = "";
            timeString += time.Minutes.ToString("0:");
            timeString += time.Seconds.ToString("00:");
            timeString += (time.Milliseconds / 10).ToString("00");
            text_TimeValue = new TextView(uiCamera, timeString, Fonts.PIXELLARI, 60, Color.White, 0, height*0.40);
            AddChild(text_TimeValue);

            menu = new SimpleMenuView(uiCamera, Fonts.DEFAULT, 30, Color.White, 35);
            menu.Y = height * 0.6;
            menu.AddOption("Play Again", () => RequestSceneSwitch(new GameScene(track)));
            menu.AddOption("Back to main menu", () => RequestSceneSwitch(new MainMenuScene()));
            AddChild(menu);

            menu.Focused = true;
        }

        protected override bool OnKeyInput(KeyEventArgs e) {
            if (e.Action == KeyAction.PRESSED) {
                if (e.Key == Keys.Escape) {
                    RequestSceneSwitch(new MainMenuScene());
                    return true;
                }
            }
            return false;
        }

    }
}
