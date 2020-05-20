using DeepFlight.generation;
using DeepFlight.gui;
using DeepFlight.network;
using DeepFlight.network.exceptions;
using DeepFlight.rendering;
using DeepFlight.scenes;
using DeepFlight.utility.KeyboardController;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Diagnostics;

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
        private TextView text_Error;

        private Track track;
        private bool online;
        private bool errorOccured = false;

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

            // Text: Displays potential error
            text_Error = new TextView(camera_UI, "", Font.DEFAULT, 24, Color.White );
            text_Error.Hidden = true;
            AddChild(text_Error);

            LoadTrack();
        }

        // Perform the actual loading
        private async void LoadTrack() {

            // IF track is null, it the "Generate Random Track" option has been selected
            if( track == null ) {
                try {
                    track = await TrackLoader.GenerateLocalTrack();
                } catch(Exception e) {
                    Trace.TraceError("Exception occured when generating track: " + e);
                    DisplayError("An error occured when generating track :(");
                    return;
                }
            }

            // If it's an online track, we should load the track data from the
            // game api. 
            if( online) {
                try {
                    var gameApi = new GameAPIConnector();
                    var blockData = await gameApi.GetTrackBlockData(track);
                    track.BlockData = blockData;
                    
                }
                catch ( APIException e) {
                    Trace.TraceError("Exception occured when loading block data from GameAPI: " + e);
                    DisplayError("An unexpected error occured when contacting game server :(");
                    return;
                }
            }

            try {
                track.DeserializeBlockData();
            }
            catch (Exception e) {
                Trace.TraceError("Exception occured when deserializing track's block data: " + e);
                DisplayError("An unexped error occured when loading track data :(");
                return;
            }

            FinishLoad(track);
        }


        private void FinishLoad(Track loadedTrack) {
            RequestSceneSwitch(new GameScene(loadedTrack, online));
        }


        // Go back to main menu on escape (only if error occurs)
        protected override bool OnKeyInput(KeyEventArgs e) {
            if (e.Action == KeyAction.PRESSED) {
                if (e.Key == Keys.Escape) {
                    if( errorOccured) {
                        RequestSceneSwitch(new MainMenuScene());
                    }
                    return true;
                }
            }
            return false;
        }


        /// <summary>
        /// Displays the error text view and sets given error message
        /// This also hides the menu and the loader in the same process
        /// </summary>
        private void DisplayError(string error) {
            errorOccured = true;
            text_Error.Text = "Error: " + error;
            text_Error.Hidden = false;
            loader.Hidden = true;
            spinningPlanet.Hidden = true;
        }
    }
}
