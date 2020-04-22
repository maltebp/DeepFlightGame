using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeepFlight.src.gui.planetbox {

    /// <summary>
    /// View to display a time paired with a label:
    /// 
    ///     Global Time
    ///      1:23.23
    /// </summary>
    class TimeLabelView : View {

        private int fontSize;
        public int FontSize {
            get => fontSize;
            set {
                fontSize = value;
                UpdateLayout();
            }
        }

        public long Time {
            set {
                if( value == 0) {
                    text_Time.Text = "No Record";
                }
                else {
                    // Build time string
                    var time = TimeSpan.FromMilliseconds(value);
                    string timeString = "";
                    if (time.Minutes > 0)
                        timeString += time.Minutes.ToString("00:");
                    timeString += time.Seconds.ToString("00:");
                    timeString += (time.Milliseconds / 10).ToString("00");
                    text_Time.Text = timeString;
                }
                
            }
        }

        public string Label {
            set => text_Label.Text = value;
        }

        public Color Color {
            set { text_Label.Color = value; text_Time.Color = value; }
        }


        public override double X { get => base.X; set { base.X = value; UpdateLayout(); } }
        public override double Y { get => base.Y; set { base.Y = value; UpdateLayout(); } }


        private TextView
            text_Label,
            text_Time;

       
        public TimeLabelView(Camera camera, string label, int x, int y, Font font, double fontSize, Color color) {
            text_Label = new TextView(camera, label, font, 0, color, x, y);
            text_Label.VOrigin = VerticalOrigin.BOTTOM;

            text_Time = new TextView(camera, "", font, 0, color, x, y);
            text_Time.VOrigin = VerticalOrigin.TOP;

            AddChildren(text_Label, text_Time);

            UpdateLayout();
        }


        // Update label and time position
        protected void UpdateLayout() {
            text_Label.Size = fontSize * 0.8;
            text_Label.Y = Y - fontSize/25;
            text_Label.X = X;

            text_Time.Size = fontSize;
            text_Time.Y = Y + fontSize/25;
            text_Time.X = X;
        }

    }
}
