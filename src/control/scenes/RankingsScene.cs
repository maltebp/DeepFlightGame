using DeepFlight.gui;
using DeepFlight.network;
using DeepFlight.network.exceptions;
using DeepFlight.rendering;
using DeepFlight.utility.KeyboardController;
using DeepFlight.view.gui;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeepFlight.scenes {
    class RankingsScene : Scene {

        private Camera camera_UI = new Camera();

        private TextView sceneTitle;

        private TextView text_Error;

        private LoadingTextView loader; 

        private RatingboardView 
            ratingboard_LastRound,
            ratingboard_Universal;

        protected override void OnInitialize() {

            // Height of UI screen
            float height = (float) ScreenController.BaseHeight;
            float width = (float) ScreenController.BaseWidth;

            // Adjust camera y=0 is top of screen
            camera_UI.Y = height / 2;

            // General background of Scene
            BackgroundColor = Settings.COLOR_PRIMARY;
            BackgroundTexture = Textures.BACKGROUND;

            sceneTitle = new TextView(camera_UI, "Online Rankings", Font.DEFAULT, 42, Color.White, 0, height * 0.15);
            AddChild(sceneTitle);

            // Text for displaying errors on center of screen
            text_Error = new TextView(camera_UI, "", Font.DEFAULT, 32, Color.White, 0, height * 0.5);
            text_Error.Hidden = true;
            AddChild(text_Error);


            ratingboard_Universal = new RatingboardView(camera_UI, "Universal Ratings (Top 5)", -width * 0.23, height * 0.55, width * 0.40f, height * 0.45f);
            ratingboard_LastRound = new RatingboardView(camera_UI, "Last Round Ratings (Top 5)", width * 0.23, height * 0.55, width * 0.40f, height * 0.45f);
            AddChildren(ratingboard_Universal, ratingboard_LastRound);

            loader = new LoadingTextView(camera_UI, y: height*0.5);
            AddChild(loader);

            LoadRatings();
        }



        /*
         * Contant the GameAPI for the current ratings */
        public async void LoadRatings() {
            ratingboard_Universal.Hidden = true;
            ratingboard_LastRound.Hidden = true;

            loader.Hidden = false;
            loader.Text = "Checking who's the very best";

            try {
                var gameApi = new GameAPIConnector();
                ratingboard_Universal.UpdateRatings(await gameApi.GetUniversalRatings(5));
                ratingboard_LastRound.UpdateRatings(await gameApi.GetRoundRatings(null, 5));
            }
            catch (ConnectionException e) {
                DisplayError("The universe seems to be offline right now :(");
                return;
            }
            catch (ServerException e) {
                DisplayError("An unknown mishap seems to have occured :(");
                return;
            }

            loader.Hidden = true;
            ratingboard_Universal.Hidden = false;
            ratingboard_LastRound.Hidden = false;

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

        private void DisplayError(string message) {
            loader.Hidden = true;
            text_Error.Text = "Error: " + message;
            text_Error.Hidden = false;
        }

    }
}
