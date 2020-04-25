using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeepFlight.gui {

    /// <summary>
    /// A simple loading view which shows a loading text, and some animating dots.
    /// </summary>
    public class LoadingTextView : View {

        private static readonly double  ANIMATE_FREQ = 0.25;

        private TextView textView;

        private string text;
        public string Text { 
            get => text;
            set {
                text = value;
                textView.Text = text;
                UpdatePosition();
            }
        }

        private double animateCooldown = ANIMATE_FREQ;
        private string dots = "";

        public LoadingTextView(Camera camera, string text = "Loading", Font font = null, double fontSize = 24, Color? color = null, double x = 0, double y = 0) : base(camera) {
            X = x;
            Y = y;
            this.text = text;
            if (font == null) font = Font.DEFAULT;
            textView = new TextView(Camera, text, font, fontSize, color.HasValue ? color.Value : Color.White, x, y);
            textView.HOrigin = HorizontalOrigin.LEFT;
            AddChild(textView);
            UpdatePosition();            
        }

        private void UpdatePosition() {
            // Make sure the text doesn't move when adding dots (while staying centered)
            textView.X = X - textView.Width / 2;
            textView.Y = Y;
        }

        protected override void OnUpdate(double deltaTime) {
            animateCooldown -= deltaTime;
            if( animateCooldown < 0) {
                dots = new string('.', (dots.Length + 1) % 4);
                textView.Text = text + dots;
                animateCooldown = ANIMATE_FREQ;
            }
        }
    }
}
