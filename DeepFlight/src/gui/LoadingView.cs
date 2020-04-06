using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeepFlight.src.gui {
    public class LoadingView : View {

        private static readonly int     FONT_SIZE = 36;
        private static readonly Font    FONT = Fonts.DEFAULT;
        private static readonly double  ANIMATE_FREQ = 0.25;

        public TextView Text { get; }

        private double animateCooldown = ANIMATE_FREQ;
        private string dots = "";

        public LoadingView(Camera camera, double x, double y) : base(camera) {
            Text = new TextView(Camera, "Loading...", FONT, FONT_SIZE, Color.White, x, y);
            AddChild(Text);

            // Make sure the text doesn't move when adding dots (while staying centered)
            Text.X -= Text.Width / 2;
            Text.HOrigin = HorizontalOrigin.LEFT;
        }

        protected override void OnUpdate(double deltaTime) {
            animateCooldown -= deltaTime;
            if( animateCooldown < 0) {
                dots = new string('.', (dots.Length + 1) % 4);
                Text.Text = "Loading" + dots;
                animateCooldown = ANIMATE_FREQ;
            }
        }

        protected override void OnDraw(Renderer renderer) {
            renderer.Draw(Camera, Text);
        }
    }
}
