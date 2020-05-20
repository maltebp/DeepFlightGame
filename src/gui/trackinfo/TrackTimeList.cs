using System;
using System.Collections.Generic;

namespace DeepFlight.src.view.gui {


    /// <summary>
    /// A Text List which displays Track Times by rows of:
    ///      [rank] [username]  [time]
    /// </summary>
    public class TrackTimeList : View {

        private static readonly int NUM_TIMES = 5;
        private static readonly int MAX_NAME_LENGTH = 7;

        private List<TimeRow> rows = new List<TimeRow>();
        
        public TrackTimeList(Camera camera, double x, double y, double fontSize, double rowSpacing, float width) : base(camera) {
            X = x;
            Y = y;

            for (int i = 0; i < NUM_TIMES; i++) {
                var row = new TimeRow(camera, i + 1, fontSize, x, y + rowSpacing * i, width);
                rows.Add(row);
                AddChild(row);
            }
        }


        /// <summary>
        /// Updates the times to display to the list of given times.
        /// The list should be sorted from best to worst.
        /// </summary>
        public void SetTimes(List<Track.Time> trackTimes) {
            int timeRank = 1;
            foreach( var time in trackTimes) {
                rows[timeRank - 1].SetTime(time);
                if (timeRank >= NUM_TIMES) break;
                timeRank++;
            }
        }


        // 3 views with player time rank, name and time
        private class TimeRow : View {

            public TextView
                text_TimeRank,
                text_Name,
                text_Time;

            private int rank;

            public TimeRow(Camera camera, int rank, double fontSize, double x, double y, float width) {

                this.rank = rank;

                // Set X position
                var leftX = x - width * 0.5;

                // Text displaying "time rank"
                text_TimeRank = new TextView(camera, "", size: fontSize*0.75);
                text_TimeRank.VOrigin = VerticalOrigin.TOP;
                text_TimeRank.HOrigin = HorizontalOrigin.LEFT;
                text_TimeRank.X = leftX;
                text_TimeRank.Y = y+2;

                // Name of given time
                text_Name = new TextView(camera, "", size: fontSize);
                text_Name.VOrigin = VerticalOrigin.TOP;
                text_Name.HOrigin = HorizontalOrigin.LEFT;
                text_Name.X = leftX + width * 0.16;
                text_Name.Y = y;

                // The actual time
                text_Time = new TextView(camera, "", size: fontSize*0.90);
                text_Time.VOrigin = VerticalOrigin.TOP;
                text_Time.HOrigin = HorizontalOrigin.RIGHT;
                text_Time.X = leftX + width;
                text_Time.Y = y;

                AddChildren(text_TimeRank, text_Name, text_Time);
            }


            public void SetTime(Track.Time trackTime) {

                text_TimeRank.Text = "#" + rank;

                // Set username
                string username = trackTime.username;
                string shortenedName = username.Length <= MAX_NAME_LENGTH ? username : username.Substring(0, MAX_NAME_LENGTH) + "..";
                text_Name.Text = shortenedName;

                // Build time string
                var time = TimeSpan.FromMilliseconds(trackTime.time);
                string timeString = "";
                if (time.Minutes > 0)
                    timeString += time.Minutes.ToString("00:");
                timeString += time.Seconds.ToString("00:");
                timeString += (time.Milliseconds / 10).ToString("00");
                text_Time.Text = timeString;
            }

        }
    }
}
