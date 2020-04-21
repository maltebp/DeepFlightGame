using DeepFlight.rendering;
using DeepFlight.gui;
using DeepFlight.utility.KeyboardController;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using DeepFlight.src.gui;
using DeepFlight.generation;
using System.Threading.Tasks;
using DeepFlight.scenes;

namespace DeepFlight.scenes {
    class TrackSelectionScene : Scene {

        private Camera ui = new Camera();
        private TextureView background;
        private SimpleMenuView menu;
        private LoadingTextView loader;
        private TextView title;

        Track[] loadedTracks = null;

        protected override void OnInitialize() {

            // Height of UI screen
            float height = (float) ScreenController.BaseHeight;
            float width = (float) ScreenController.BaseWidth;

            // Adjust camera y=0 is top of screen
            ui.Y = height / 2;

            BackgroundColor = Settings.COLOR_PRIMARY;

            title = new TextView(ui, "Tracks", Font.DEFAULT, 42, Color.White, 0, height * 0.20);
            AddChild(title);

            menu = new SimpleMenuView(ui, Font.DEFAULT, 30, Color.White, 35);
            menu.Y = height * 0.35;
            AddChild(menu);

            loader = new LoadingTextView(ui, y: height / 2);
            loader.Text = "Loading tracks";
            AddChild(loader);

            LoadTracks();
            Console.WriteLine("Started track loading!");
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

        private async void LoadTracks() {
            menu.Hidden = true;
            loader.Hidden = false;

            var generateTrackTask = TrackLoader.GenerateLocalTrack();
            var track = await generateTrackTask;

            Console.WriteLine("Track generated: " + track);

            loadedTracks = new Track[]{ track };
        }

        protected override void OnUpdate(double deltaTime) {
            if( loadedTracks != null) {
                TracksLoaded(loadedTracks);
                loadedTracks = null;
            }
        }

        private void TracksLoaded(params Track[] tracks) {
            foreach (var track in tracks) {
                menu.AddSimpleOption(track.Planet.Name + ": " + track.Name, () => TrackSelected(track));
            }

            menu.Hidden = false;
            menu.Focused = true;    
            loader.Hidden = true;
        }


        private void TrackSelected(Track track) {
            Console.WriteLine("Track Selected: " + track);
            track.DeserializeBlockData();
            RequestSceneSwitch(new GameScene(track));
        }



        
    }
}
