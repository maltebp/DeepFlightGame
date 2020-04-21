using DeepFlight.gui;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeepFlight.src.gui {


    /// <summary>
    /// A PlanetBoxView to display information about a Track, and as such
    /// the color and text content is dependent on the Track. 
    /// </summary>
    public class TrackInfoView : PlanetBoxView {

        private Track track;
        public Track Track {
            get => track;
            set {
                track = value;
                FocusColor = track.Planet.Color;
                text_TrackName.Text = track.Name;
                text_PlanetName.Text = track.Planet.Name;
            }
        }

        private TextView
            text_PlanetName,
            text_TrackName;
            //text_TimeLabel,
            //text_TimeValue;

        public TrackInfoView(Camera camera, Track track) : base(camera, focusColor: track.Planet.Color) {
            text_PlanetName = new TextView(camera, "", Font.DEFAULT, 1, Color.White, 0, 0);
            text_TrackName = new TextView(camera, "", Font.DEFAULT, 1, Color.White, 0, 0);

            // TODO: Remove text_TimeLabel and text_TimeValue, if I don't have time to implement local time 
            //text_TimeLabel  = new TextView(camera, "-", Fonts.DEFAULT, 1, Color.White, 0, 0);
            //text_TimeValue   = new TextView(camera, "", Fonts.DEFAULT, 1, Color.White, 0, 0);

            //AddChildren(text_PlanetName, text_TrackName, text_TimeLabel, text_TimeValue);
            AddChildren(text_PlanetName, text_TrackName);

            Track = track;
            UpdateLayout();
        }
     

        protected override void UpdateLayout() {
            base.UpdateLayout();
            
            var fontColor = Focused ? Color.White : (track != null ? track.Planet.Color : Color.DarkGray);
            double centerX = GetCenterX();
           
            double focusScale = (Focused) ? FocusScale : 1;
            double scaledSize = focusScale * Size;
            double topY = Y - scaledSize / 2.0;

            if ( text_PlanetName != null) {
                text_PlanetName.Y = topY + scaledSize * 0.35;
                text_PlanetName.X = centerX;
                text_PlanetName.Size = 25 * focusScale; // Font Size
                text_PlanetName.Color = fontColor;
            }

            if (text_TrackName != null) {
                text_TrackName.Y = topY + scaledSize * 0.60;
                text_TrackName.X = centerX;
                text_TrackName.Size = 30 * focusScale; // Font 
                text_TrackName.Color = fontColor;
            }

            //if (text_TimeLabel != null) {
            //    Console.WriteLine("Time label: " + text_TimeLabel);
            //    text_TimeLabel.Y = topY + scaledSize * 0.40;
            //    text_TimeLabel.X = centerX;
            //    text_TimeLabel.Size = 20 * focusScale; // Font Size
            //    text_TimeLabel.Color = fontColor;
            //    text_TimeLabel.Text = "Best time: ";
            //}

            //if (text_TimeValue != null) {
            //    text_TimeValue.Y = topY + scaledSize * 0.80;
            //    text_TimeValue.X = centerX;
            //    text_TimeValue.Size = 30 * focusScale; // Font Size
            //    text_TimeValue.Color = fontColor;
            //}
        }
    }
}
