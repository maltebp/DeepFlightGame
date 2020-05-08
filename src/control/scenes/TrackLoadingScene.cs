using DeepFlight.generation;
using DeepFlight.gui;
using DeepFlight.network;
using DeepFlight.rendering;
using DeepFlight.scenes;
using Microsoft.Xna.Framework;

namespace DeepFlight.src.scenes {


    /// <summary>
    /// Since which loads a specified Tracks by either generating it
    /// or deserializing the blocks depending on construct argument.
    /// The scene then starte the Game scene with that track.
    /// </summary>
    public class TrackLoadingScene : Scene {

        private Camera camera_UI = new Camera();
        private PlanetBoxView spinningPlanet;
        private LoadingTextView loader;

        private Track track;
        private bool online;

        /// <summary>
        /// Initializes the track loading scene.
        /// </summary>
        /// <param name="track">If null, it generates a new track rather than loading</param>
        public TrackLoadingScene(Track track, bool online) {
            this.track = track;
            this.online = online;
        }

        protected override void OnInitialize() {

            var height = ScreenController.BaseHeight;

            BackgroundColor = Settings.COLOR_PRIMARY;
            BackgroundTexture = Textures.BACKGROUND;

            // Setup the spinning planet
            spinningPlanet = new PlanetBoxView(camera_UI, unfocusColor: track != null ? track.Planet.Color : Color.White);
            spinningPlanet.Y = -height * 0.10;
            spinningPlanet.RotationVelocity = 3.0f;

            // Setup loading text
            loader = new LoadingTextView(camera_UI, y: height*0.10);
            if (track != null) loader.Text = string.Format("Travelling to cave {0} from planet {1}", track.Name, track.Planet.Name);
            else loader.Text = string.Format("Finding an unused cave from some planet");

            AddChildren(spinningPlanet, loader);

            LoadTrack();
        }

        // Perform the actual loading
        private async void LoadTrack() {
            if( track == null ) {
                track = await TrackLoader.GenerateLocalTrack();
            }
            if( online) {
                // TODO: Add try catch here
                var gameApi = new GameAPIConnector();
                var blockData = await gameApi.GetTrackBlockData(track);
                track.BlockData = blockData;
            }
            await track.DeserializeBlockDataAsync();
            FinishLoad(track);
        }


        private void FinishLoad(Track loadedTrack) {
            RequestSceneSwitch(new GameScene(loadedTrack, online));
        }
    }
}
