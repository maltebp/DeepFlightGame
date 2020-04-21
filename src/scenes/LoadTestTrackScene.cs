
using DeepFlight.gui;
using DeepFlight.generation;


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

            loader = new LoadingTextView(ui);
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
