
using System; 
using DeepFlight.gui;
using DeepFlight.network;
using DeepFlight.network.exceptions;
using DeepFlight.rendering;
using DeepFlight.src.gui;
using DeepFlight.src.scenes;
using DeepFlight.track;
using DeepFlight.utility.KeyboardController;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;



namespace DeepFlight.scenes {
    public class OnlineTracksScene : Scene {

        // Create camera which has its origin (0,0) in the upperleft corner
        private Camera camera_UI = new Camera(x: ScreenController.BaseWidth/2, y: ScreenController.BaseHeight/2);

        private TextView text_SceneTitle;

        private MenuView menu_Tracks;
        private TextView text_Error;
        private LoadingTextView loader;

        private Round round;

        protected override void OnInitialize() {

            float height = (float) ScreenController.BaseHeight;
            float width = (float) ScreenController.BaseWidth;

            BackgroundColor = Settings.COLOR_PRIMARY;
            BackgroundTexture = Textures.BACKGROUND;

            text_SceneTitle = new TextView(camera_UI, "<ROUND TITLE>", Font.DEFAULT, 42, Color.White, width * 0.50, height * 0.20);
            text_SceneTitle.Hidden = true;
            AddChild(text_SceneTitle);

            // Menu which contains the different Tracks
            menu_Tracks = new MenuView(orientation: MenuView.MenuOrientation.HORIZONTAL);
            menu_Tracks.Hidden = true;
            AddChild(menu_Tracks);

            // TextView for displaing potential error
            text_Error = new TextView(camera_UI, "", Font.DEFAULT, 24, Color.White, width*0.5, height * 0.5);
            text_Error.Hidden = true;
            AddChild(text_Error);

            // Loader 
            loader = new LoadingTextView(camera_UI, "Loading tracks for current round", Font.DEFAULT, 24, Color.White, width*0.5, height*0.5);
            loader.Hidden = true;
            AddChild(loader);

            LoadTracks();
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


        private async void LoadTracks() {
            loader.Hidden = false;

            // Load current round

            try {
                var gameApi = new GameAPIConnector();
                round = await gameApi.GetCurrentRound();
            } catch(ConnectionException e) {
                DisplayError("The universe seems to be offline right now :(");
                return;
            } catch(ServerException e) {
                DisplayError("An unknown mishap seems to have occured :(");
                return;
            }

            int count = 0;
            foreach (var track in round.Tracks) {
                TrackInfoView planet = new TrackInfoView(camera_UI, track, true);
                planet.FocusColor = track.Planet.Color;
                planet.X = ScreenController.BaseWidth * (count + 1) * 0.20;
                planet.Y = ScreenController.BaseHeight * 0.50;
                menu_Tracks.AddMenuOption(planet, () => RequestSceneSwitch(new TrackLoadingScene(track, true)));
                count++;
            }

            loader.Hidden = true;

            menu_Tracks.Hidden = false;
            menu_Tracks.Focused = true;

            text_SceneTitle.Hidden = false;
            text_SceneTitle.Text = string.Format("Round #{0} Tracks", round.RoundNumber);
        }

        private void DisplayError(string errorMessage) {
            text_Error.Text = "Error: " + errorMessage;
            text_Error.Hidden = false;
            menu_Tracks.Hidden = true;
            loader.Hidden = true;
        }

    }
}
