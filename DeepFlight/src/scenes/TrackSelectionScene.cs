using DeepFlight.rendering;
using DeepFlight.gui;
using DeepFlight.utility.KeyboardController;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using DeepFlight.src.gui;

namespace DeepFlight.src.scenes {
    class TrackSelectionScene : Scene {


        private Camera ui = new Camera();
        private TextureView background;
        private SimpleMenuView menu;
        private LoadingTextView loader;
        private TextView title;

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

            title = new TextView(ui, "Tracks", Fonts.DEFAULT, 42, Color.White, 0, height * 0.20);
            AddChild(title);

            menu = new SimpleMenuView(ui, Fonts.DEFAULT, 30, Color.White, 35);
            menu.Y = height * 0.35;
            AddChild(menu);

            loader = new LoadingTextView(ui, Fonts.DEFAULT, 24, Color.White, 0, height / 2);
            loader.Text = "Loading tracks";
            AddChild(loader);

            LoadTracks();
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



        // TODO: Remove this once authentication has been implemented
        private bool loading = false;
        private double loadTimer = 1.5;
        protected override void OnUpdate(double deltaTime) {
            if (loading) {
                loadTimer -= deltaTime;
                if (loadTimer < 0) {
                    loading = false;
                    Track[] tracks = new Track[4];
                    for(int i = 0; i < 4; i++) {
                        tracks[i] = new Track();
                        tracks[i].Name = "Track " + i;
                    }
                    TracksLoaded(tracks);
                }
            }
        }

        private void LoadTracks() {
            menu.Hidden = true;
            loader.Hidden = false;
            loading = true;

        }

        private void TracksLoaded(params Track[] tracks) {

            foreach (var track in tracks) {
                menu.AddOption(track.Name, () => TrackSelected(track));
            }

            menu.Hidden = false;
            menu.Focused = true;
            loader.Hidden = true;

            menu.Test();
        }

        private void TrackSelected(Track track) {
            Console.WriteLine("Track Selected: " + track);
        }

        
    }
}
