using DeepFlight.generation;
using DeepFlight.gui;
using DeepFlight.rendering;
using DeepFlight.scenes;
using DeepFlight.src.network;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeepFlight.src.scenes {


    /// <summary>
    /// Since which loads a specified Tracks by either generating it
    /// or deserializing the blocks depending on construct argument.
    /// The scene then starte the Game scene with that track.
    /// </summary>
    public class TrackLoadingScene :Scene {

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

            // Setup the spinning planet
            spinningPlanet = new PlanetBoxView(camera_UI, unfocusColor: track != null ? track.Planet.Color : Color.White);
            spinningPlanet.Y = -height * 0.10;
            spinningPlanet.RotationVelocity = 3.0f;

            // Setup loading text
            loader = new LoadingTextView(camera_UI, y: height*0.10);
            if (track != null) loader.Text = string.Format("Loading cave {0} from planet {1}", track.Name, track.Planet.Name);
            else loader.Text = string.Format("Finding an unused cave from some planet");

            AddChildren(spinningPlanet, loader);

            LoadTrack();
        }


        protected override void OnUpdate(double deltaTime) {
            Mover.Move(spinningPlanet, deltaTime);
        }


        // Perform the actual loading
        private async void LoadTrack() {
            if( track == null ) {
                track = await TrackLoader.GenerateLocalTrack();
            }
            if( online) {
                var gameApi = new GameAPIConnector();
                await gameApi.GetTrackBlockData(track);
            }
            await track.DeserializeBlockDataAsync();
            FinishLoad(track);
        }


        private void FinishLoad(Track loadedTrack) {
            RequestSceneSwitch(new GameScene(loadedTrack));
        }
    }
}
