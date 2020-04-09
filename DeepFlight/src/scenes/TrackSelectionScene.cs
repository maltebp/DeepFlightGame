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

            Task<Track[]> loadTracksTask = TrackLoader.LoadTracks();
            Track[] tracks = await loadTracksTask;
            Console.WriteLine("Tracks loaded: ");

            foreach(var track in tracks) {
                Console.WriteLine("\t" + track);
            }

            loadedTracks = tracks;
        }

        protected override void OnUpdate(double deltaTime) {
            if( loadedTracks != null) {
                TracksLoaded(loadedTracks);
                loadedTracks = null;
            }
        }

        private void TracksLoaded(params Track[] tracks) {
            foreach (var track in tracks) {
                menu.AddOption(track.Name, () => TrackSelected(track));
            }

            menu.Hidden = false;
            menu.Focused = true;    
            loader.Hidden = true;
        }


        private void TrackSelected(Track track) {
            Console.WriteLine("Track Selected: " + track);
            RequestSceneSwitch(new GameScene(track));
        }



        
    }
}
