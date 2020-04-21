using DeepFlight.generation;
using DeepFlight.gui;
using DeepFlight.rendering;
using DeepFlight.src.gui;
using DeepFlight.src.scenes;
using DeepFlight.utility.KeyboardController;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;


namespace DeepFlight.scenes {
    public class OfflineTracksScene : Scene {

        // Create camera which has its origin (0,0) in the upperleft corner
        private Camera camera_UI = new Camera(x: ScreenController.BaseWidth/2, y: ScreenController.BaseHeight/2);

        private TextView text_SceneTitle;

        private MenuView menu_Tracks;
        private TextView text_Error;
        private LoadingTextView loader;

        private Track[] offlineTracks = null;

        protected override void OnInitialize() {

            float height = (float) ScreenController.BaseHeight;
            float width = (float) ScreenController.BaseWidth;

            BackgroundColor = Settings.COLOR_PRIMARY;

            text_SceneTitle = new TextView(camera_UI, "Offline Tracks", Font.DEFAULT, 42, Color.White, width * 0.50, height * 0.20);
            AddChild(text_SceneTitle);

            // Menu which contains the different Tracks
            menu_Tracks = new MenuView(orientation: MenuView.MenuOrientation.HORIZONTAL);
            menu_Tracks.Hidden = true;
            AddChild(menu_Tracks);

            // TextView for displaing potential error
            text_Error = new TextView(camera_UI, "", Font.DEFAULT, 24, Color.White, width*0.5, height * 0.75);
            text_Error.Hidden = true;
            AddChild(text_Error);

            // Loader 
            loader = new LoadingTextView(camera_UI, "Loading offline tracks", Font.DEFAULT, 24, Color.White, width*0.5, height*0.5);
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

            offlineTracks = await TrackLoader.LoadOfflineTracks();

            loader.Hidden = true;

            if( offlineTracks.Length == 0) {
                DisplayError("Couldn't load offline tracks");
            }
            else {
                if (offlineTracks.Length != 3) {
                    Console.WriteLine("WARNING: Expected 3 offline Tracks, but got " + offlineTracks.Length);
                }

                // Create planet views

                int count = 0;
                foreach(var track in offlineTracks) {
                    TrackInfoView planet = new TrackInfoView(camera_UI, track);
                    planet.FocusColor = track.Planet.Color;
                    planet.X = ScreenController.BaseWidth * (count + 1) * 0.20;
                    planet.Y = ScreenController.BaseHeight * 0.50;
                    menu_Tracks.AddMenuOption(planet, () => RequestSceneSwitch(new TrackLoadingScene(track, false)));
                    count++;
                }
            }
            TextPlanetBoxView randomPlanet = new TextPlanetBoxView(camera_UI, "Random Track");
            randomPlanet.FocusColor = Color.White;
            randomPlanet.X = ScreenController.BaseWidth * 0.80;
            randomPlanet.Y = ScreenController.BaseHeight * 0.50;
            menu_Tracks.AddMenuOption(randomPlanet, () => RequestSceneSwitch(new TrackLoadingScene(null, false)));

            menu_Tracks.Hidden = false;
            menu_Tracks.Focused = true;
        }

        private void DisplayError(string errorMessage) {
            text_Error.Text = "Error: " + errorMessage;
            text_Error.Hidden = false;
        }

    }
}
