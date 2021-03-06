﻿using DeepFlight.gui;
using DeepFlight.network;
using DeepFlight.network.exceptions;
using DeepFlight.rendering;
using DeepFlight.user;
using DeepFlight.utility.KeyboardController;
using DeepFlight.view.gui;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;


namespace DeepFlight.scenes {
    
    /// <summary>
    /// Scene displaying the current Universal Ranking, as well as the rankings
    /// of the previous Round.
    /// </summary>
    class RankingsScene : Scene {

        private Camera camera_UI = new Camera();

        private TextView sceneTitle;

        private TextView text_Error;

        private TextView text_RoundRankingsMessage;

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

            // Ratingboards: Displays Universal and Round rankings
            ratingboard_Universal = new RatingboardView(camera_UI, "Universal Rankings (Top 5)", -width * 0.23, height * 0.55, width * 0.40f, height * 0.45f);
            ratingboard_LastRound = new RatingboardView(camera_UI, "Previous Round Ratings (Top 5)", width * 0.23, height * 0.55, width * 0.40f, height * 0.45f);
            AddChildren(ratingboard_Universal, ratingboard_LastRound);

            // Text for displaying message regarding round ranking
            text_RoundRankingsMessage = new TextView(camera_UI, "", x: ratingboard_LastRound.X, y: ratingboard_LastRound.Y);
            text_RoundRankingsMessage.Hidden = true;
            AddChild(text_RoundRankingsMessage);

            loader = new LoadingTextView(camera_UI, y: height*0.5);
            AddChild(loader);

            LoadRatings();
        }



        /*
         * Contant the GameAPI for the current ratings */
        public async void LoadRatings() {

            // Start loading
            ratingboard_Universal.Hidden = true;
            ratingboard_LastRound.Hidden = true;
            loader.Hidden = false;
            loader.Text = "Checking who's the very best";

            try {
                // Fetch rankings from GameAPI
                var gameApi = new GameAPIConnector();
                var universalRankings = await gameApi.GetUniversalRankings();
                var lastRound = await gameApi.GetPreviousRound();

                // Hide and shows stuff
                loader.Hidden = true;
                ratingboard_Universal.Hidden = false;
                ratingboard_LastRound.Hidden = false;

                // Universal ranking
                ratingboard_Universal.UpdateRankings(universalRankings);
                if( User.LocalUser.Guest) {
                    ratingboard_Universal.HideUserRanking();
                }
                else {
                    // Show user's ranking
                    foreach (var ranking in universalRankings) {
                        if (ranking.name == User.LocalUser.Username) {
                            ratingboard_Universal.UpdateUserRanking(ranking);
                            break;
                        }
                    }
                }

                // Previous Round rankings
                if (lastRound == null) {
                    text_RoundRankingsMessage.Text = "No previous round exists";
                    text_RoundRankingsMessage.Hidden = false;
                    ratingboard_LastRound.HideRankings();
                }
                else {
                    ratingboard_LastRound.Title = $"Round #{lastRound.RoundNumber} rankings";
                    if (lastRound.Rankings == null) {
                        text_RoundRankingsMessage.Text = $"Round hasn't been ranked yet";
                        text_RoundRankingsMessage.Hidden = false;
                        ratingboard_LastRound.HideRankings();
                    }
                    else {
                        // Rankings are available, so we update them
                        ratingboard_LastRound.UpdateRankings(lastRound.Rankings);
                        if (User.LocalUser.Guest) {
                            ratingboard_LastRound.HideUserRanking();
                        }
                        else {
                            // Display the user's ranking
                            foreach(var ranking in lastRound.Rankings) {
                                if( ranking.name == User.LocalUser.Username) {
                                    ratingboard_LastRound.UpdateUserRanking(ranking);
                                    break;
                                }
                            }
                        }
                    }
                }
            }
            catch (ConnectionException e) {
                DisplayError("The universe seems to be offline right now :(");
            }
            catch (ServerException e) {
                DisplayError("An unknown mishap seems to have occured :(");
            }
        }
        

        // Displays an error on the screen
        private void DisplayError(string message) {
            loader.Hidden = true;
            text_Error.Text = "Error: " + message;
            text_Error.Hidden = false;
        }


        // Exit to main menu on escape pressed
        protected override bool OnKeyInput(KeyEventArgs e) {
            if (e.Action == KeyAction.PRESSED) {
                if (e.Key == Keys.Escape) {
                    RequestSceneSwitch(new MainMenuScene());
                    return true;
                }
            }
            return false;
        }

    }
}
