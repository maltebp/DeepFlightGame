using DeepFlight.gui;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeepFlight.src.gui {

    /// <summary>
    /// PlanetBoxView with a single line of text
    /// </summary>
    public class TextPlanetBoxView : PlanetBoxView {

        private TextView textView;

        public TextPlanetBoxView(Camera camera, string text) : base(camera) {
            textView = new TextView(camera, text, Font.DEFAULT, 1, Color.DarkGray, 0, 0);
            AddChild(textView);
            UpdateLayout();
        }

        // Update text position and scaling
        protected override void UpdateLayout() {
            base.UpdateLayout();
            double focusScale = (Focused) ? FocusScale : 1;

            if ( textView != null) {
                textView.Y = GetCenterY();
                textView.X = GetCenterX();
                textView.Size = 22 * focusScale; // Font Size
            }
        }
    }
}
