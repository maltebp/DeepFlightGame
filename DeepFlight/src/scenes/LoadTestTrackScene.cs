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

    /// <summary>
    /// Simple Scene for testing, which loads a pregenereated test Track file
    /// named 'testtrack.dft', and starts the GameScene.
    /// </summary>
    class LoadTestTrackScene : Scene {

        private Camera ui = new Camera();
        private LoadingTextView loader;

        protected override void OnInitialize() {
            BackgroundColor = Settings.COLOR_PRIMARY;

            loader = new LoadingTextView(ui, Fonts.DEFAULT, 24, Color.White, 0, 0);
            loader.Text = "Loading Test Track...";
            AddChild(loader);

            LoadTracks();
        }

        private async void LoadTracks() {
            Track track = await TrackLoader.LoadTrackFileAsync("testtrack.dft");
            loader.Text = "Deserializing blocks...";
            await track.DeserializeBlockDataAsync();
            RequestSceneSwitch(new GameScene(track));
        }
        
    }
}
