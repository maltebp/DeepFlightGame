using DeepFlight.gui;
using DeepFlight.src.gui.planetbox;
using DeepFlight.src.view.gui;
using DeepFlight.user;
using Microsoft.Xna.Framework;
using System.Linq;

namespace DeepFlight.src.gui {


    /// <summary>
    /// A PlanetBoxView to display information about a Track, and
    /// the color and text content is dependent on the Track. 
    /// </summary>
    public class TrackInfoView : PlanetBoxView {

        private Track track;
        public Track Track {
            get => track;
            set {
                track = value;
                UpdateTrackInfo();
            }
        }

        private TextView
            text_PlanetName,
            text_TrackName;

        private TimeLabelView
            time_GlobalBest,
            time_PersonalBest;

        private TrackTimeList timeList;

        private bool displayOnlineInfo = false;

        public TrackInfoView(Camera camera, Track track, double x, double y, bool displayOnlineInfo) : base(camera, focusColor: track.Planet.Color) {
            this.displayOnlineInfo = displayOnlineInfo;
            X = x;
            Y = y;

            FocusColor = track.Planet.Color;

            text_PlanetName = new TextView(camera, "", Font.DEFAULT, 1, Color.White, 0, 0);
            text_TrackName = new TextView(camera, "", Font.DEFAULT, 1, Color.White, 0, 0);
            AddChildren(text_PlanetName, text_TrackName);

            if (displayOnlineInfo) {
                time_GlobalBest = new TimeLabelView(camera, "Global Best:", 0, 0, Font.DEFAULT, 0, Color.White);
                AddChild(time_GlobalBest);

                timeList = new TrackTimeList(camera, x, y+150, 17, 30, 190);
                timeList.SetTimes(track.Times);
                timeList.Hidden = true;
                AddChild(timeList); 
            }
            time_PersonalBest = new TimeLabelView(camera, "Personal Best:", 0, 0, Font.DEFAULT, 0, Color.White);
            AddChild(time_PersonalBest);

            Track = track;
            UpdateTrackInfo();
            UpdateLayout();
        }

        protected override void OnFocus() {
            base.OnFocus();
            if( timeList != null )
                timeList.Hidden = false;
        }

        protected override void OnUnfocus() {
            base.OnUnfocus();
            if( timeList != null )
                timeList.Hidden = true;
        }


        private void UpdateTrackInfo() {

            // Update information
            FocusColor = track.Planet.Color;
            text_TrackName.Text = track.Name;
            text_PlanetName.Text = track.Planet.Name;

            // Update Global time
            if( time_GlobalBest != null ) {
                time_GlobalBest.Time = 0;
                if( track.Times.Count > 0 ) {
                    time_GlobalBest.Time = track.Times.First().time;
                }
            }

            time_PersonalBest.Time = 0;
            // Update User's best
            if (time_GlobalBest == null ) {
                time_PersonalBest.Time = track.OfflineTime;
            }
            else {
                foreach (var trackTime in track.Times) {
                    if (trackTime.username == User.LocalUser.Username) {
                        time_PersonalBest.Time = trackTime.time;
                    }
                }
            }
            

        }


        protected override void UpdateLayout() {
            base.UpdateLayout();

            var fontColor = Focused ? Color.White : (track != null ? track.Planet.Color : Color.DarkGray);
            double centerX = GetCenterX();

            double focusScale = (Focused) ? FocusScale : 1;
            double scaledSize = focusScale * Size;
            double topY = Y - scaledSize / 2.0;

            if (text_PlanetName != null) {
                text_PlanetName.Y = topY + scaledSize * 0.15;
                text_PlanetName.X = centerX;
                text_PlanetName.Size = 19 * focusScale; // Font Size
                text_PlanetName.Color = fontColor;
            }

            if (text_TrackName != null) {
                text_TrackName.Y = topY + scaledSize * 0.35;
                text_TrackName.X = centerX;
                text_TrackName.Size = 26 * focusScale; // Font 
                text_TrackName.Color = fontColor;
            }

            if (time_PersonalBest != null) {
                time_PersonalBest.FontSize = (int)(19 * focusScale);
                time_PersonalBest.Color = fontColor;
                time_PersonalBest.X = centerX;
                time_PersonalBest.Y = topY + scaledSize * 0.65;
            }

            if (time_GlobalBest != null) {
                time_GlobalBest.FontSize = (int)(17 * focusScale);
                time_GlobalBest.Color = fontColor;
                time_GlobalBest.X = centerX;
                time_GlobalBest.Y = topY + scaledSize * 0.57;
                time_PersonalBest.Y = topY + scaledSize * 0.82;
                time_PersonalBest.FontSize = (int)(17 * focusScale);
            }
        }
    }
}
