﻿using DeepFlight.control.offlinetracktime;
using DeepFlight.gui;
using DeepFlight.network;
using DeepFlight.network.exceptions;
using DeepFlight.rendering;
using DeepFlight.src.gui;
using DeepFlight.user;
using DeepFlight.utility.KeyboardController;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Diagnostics;

namespace DeepFlight.scenes {

    /// <summary>
    /// Displays the result when a track has been completed.
    /// The result will be uploaded to the game server, and
    /// tested whether or not it's a recrod.
    /// </summary>
    class TrackCompleteScene : Scene {

        private Camera uiCamera = new Camera();
        private SimpleMenuView menu;
        private TextView text_TrackComplete;
        private TextView text_TimeText;
        private TextView text_TimeValue;
        private TextView text_Result;

        private LoadingTextView loader;

        private Track track;
        private TimeSpan time;
        private bool onlineTrack;

        /// <param name="time"> The track time (how long it took the user took complete) </param>
        /// <param name="onlineTrack"> Whether or not this is an online track (determines how the time should be updated)</param>
        public TrackCompleteScene(Track track, TimeSpan time, bool onlineTrack) {
            this.track = track;
            this.time = time;
            this.onlineTrack = onlineTrack;
        }

        protected override void OnInitialize() {

            // Height of UI screen
            float height = (float)ScreenController.BaseHeight;
            float width = (float)ScreenController.BaseWidth;

            // Adjust camera y=0 is top of screen
            uiCamera.Y = height / 2;

            // Create background
            BackgroundColor = Settings.COLOR_PRIMARY;
            BackgroundTexture = Textures.BACKGROUND;

            // Text: Scene title
            text_TrackComplete = new TextView(uiCamera, "Track Complete!", Font.PIXELLARI, 40, Color.White, 0, height*0.15);
            AddChild(text_TrackComplete);

            // Text: Displays the track time
            text_TimeText = new TextView(uiCamera, "Your time:", Font.PIXELLARI, 24, Color.White, 0, height*0.30 );
            AddChild(text_TimeText);            
            var timeString = "";
            timeString += time.Minutes.ToString("0:");
            timeString += time.Seconds.ToString("00:");
            timeString += (time.Milliseconds / 10).ToString("00");
            text_TimeValue = new TextView(uiCamera, timeString, Font.PIXELLARI, 60, Color.White, 0, height*0.40);
            AddChild(text_TimeValue);

            // Text: Displays whether or not new time was a record
            text_Result = new TextView(uiCamera, "<RESULT>", Font.DEFAULT, 30, Color.White, 0, height * 0.60);
            AddChild(text_Result);

            // Menu: With options to go back to main menu or play again
            menu = new SimpleMenuView(uiCamera, Font.DEFAULT, 24, Color.White, 35);
            menu.Y = height * 0.75;
            menu.AddSimpleOption("Play Again", () => RequestSceneSwitch(new GameScene(track, onlineTrack)));
            menu.AddSimpleOption("Back to main menu", () => RequestSceneSwitch(new MainMenuScene()));
            menu.Focused = false;
            AddChild(menu);

            // Loader
            loader = new LoadingTextView(uiCamera, y: height * 0.5);
            loader.Hidden = true;
            AddChild(loader);

            if (onlineTrack)
                UploadTime();
            else
                CheckLocalRecord();
        }


        // Upload the time to the GameAPI and check for personal record
        private async void UploadTime() {
            menu.Hidden = true;
            text_Result.Hidden = true;
            loader.Hidden = false;
            loader.Text = "Checking for a new record";

            bool newRecord;
            try {
                var gameApi = new GameAPIConnector();
                newRecord = await gameApi.UpdateUserTrackTime(User.LocalUser, track, (ulong)time.TotalMilliseconds);
            } catch(APIException e) {
                Trace.TraceError("Exception when Updating user track time: " + e);
                DisplayError("Something went wrong, and your time is lost :(");
                return;
            }
        
            if (newRecord) DisplayResult("New personal record!");
            else DisplayResult("You've done better :(");
        }


        // Checks if the time was a local record
        // (is only called if it was an online track)
        private void CheckLocalRecord() {
            var timeController = OfflineTrackTimeController.Instance;
            var newRecord = timeController.UpdateTrackTime(track.Name, (long) time.TotalMilliseconds);

            if( newRecord) {
                DisplayResult("New personal record!");
            }
            else {
                DisplayResult("You've done better :(");
            }

        }

        private void DisplayError(string error) {
            DisplayResult("Error: "+error);
        }

        private void DisplayResult(string result) {
            menu.Hidden = false;
            menu.Focused = true;

            /* Due to the way things are drawn, the simple menu options bug out,
             * causing them to be drawn while not focused.
             * This is a bad fix, but it's a fix nonetheless
             */
            menu.FocusedOptionIndex = 1;
            menu.FocusedOptionIndex = 0;

            text_Result.Hidden = false;
            loader.Hidden = true;
            text_Result.Text = result;
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

    }
}
