﻿
using System; 
using DeepFlight.gui;
using DeepFlight.network;
using DeepFlight.network.exceptions;
using DeepFlight.rendering;
using DeepFlight.src.gui;
using DeepFlight.src.scenes;
using DeepFlight.track;
using DeepFlight.user;
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
        private TextView text_TimeLeft;
        private LoadingTextView loader;

        private Round round;

        private bool loadingTracks = false;

        protected override void OnInitialize() {

            float height = (float) ScreenController.BaseHeight;
            float width = (float) ScreenController.BaseWidth;

            BackgroundColor = Settings.COLOR_PRIMARY;
            BackgroundTexture = Textures.BACKGROUND;

            // Text: Round title ("scene title")
            text_SceneTitle = new TextView(camera_UI, "<ROUND TITLE>", Font.DEFAULT, 42, Color.White, width * 0.50, height * 0.20);
            text_SceneTitle.Hidden = true;
            AddChild(text_SceneTitle);

            // Text: Time left of round 
            text_TimeLeft = new TextView(camera_UI, "", x: width * 0.5, y: height*0.28);
            text_TimeLeft.Hidden = false;
            AddChild(text_TimeLeft);

            // Menu: Contains the different Tracks (tracks are added once loaded)
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

            // Guest user shouldn't be allowed to play the online tracks
            if (User.LocalUser.Guest) {
                DisplayError("Sorry, you cannot play the online tracks as a guest!");
            }
            else {
                LoadTracks();
            }

        }


        // Loads the tracks asynchronously, and creates the Track info views
        // once finished, and adds them to the menu.
        private async void LoadTracks() {
            loader.Hidden = false;
            loadingTracks = true;

            // Load current round from GameAPI
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

            // Check if a round even exists
            if( round == null ) {
                DisplayError("No round is active at the moment - come back later!");
                return;
            }

            menu_Tracks.Hidden = false;

            // Create Track Info views (the planets)
            int count = 0;
            foreach (var track in round.Tracks) {
                TrackInfoView planet = new TrackInfoView(
                    camera_UI,
                    track,
                    ScreenController.BaseWidth * (count + 1) * 0.20,
                    ScreenController.BaseHeight * 0.55,
                    true
                );
               
                menu_Tracks.AddMenuOption(planet, () => RequestSceneSwitch(new TrackLoadingScene(track, true)));
                count++;
            }

            loader.Hidden = true;
            menu_Tracks.Focused = true;
            text_TimeLeft.Hidden = false;
            text_SceneTitle.Hidden = false;
            text_SceneTitle.Text = $"Round #{round.RoundNumber} Tracks";
            loadingTracks = false;
        }


        // Update the round countdown
        protected override void OnUpdate(double deltaTime) {
            if( round != null) {
                var remainingTimeMs = round.EndDate - DateTimeOffset.Now.ToUnixTimeMilliseconds();
                if (remainingTimeMs < 0) remainingTimeMs = 0;
                var remainingTime = TimeSpan.FromMilliseconds(remainingTimeMs);
                var timeString = "";
                timeString += remainingTime.Hours.ToString("00:");
                timeString += remainingTime.Minutes.ToString("00:");
                timeString += remainingTime.Seconds.ToString("00");
                text_TimeLeft.Text = "Time left: " + timeString;

            }       
        }


        // Displays an error, hiding the Track menu
        private void DisplayError(string errorMessage) {
            text_Error.Text = "Error: " + errorMessage;
            text_Error.Hidden = false;
            menu_Tracks.Hidden = true;
            loader.Hidden = true;
        }


        // Return to Main menu on exit
        protected override bool OnKeyInput(KeyEventArgs e) {
            if (e.Action == KeyAction.PRESSED) {
                if (e.Key == Keys.Escape) {
                    if( !loadingTracks) {
                        RequestSceneSwitch(new MainMenuScene());
                        return true;
                    }
                }
            }
            return false;
        }

    }
}
