using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeepFlight.view.gui {
    public class BorderView : View {

        public float borderWidth = 1f;
        public float BorderWidth { get => borderWidth; set { borderWidth = value; UpdateLayout(); } }

        // Overrides in order to update children layout
        public override double X { get => base.X; set { base.X = value; UpdateLayout(); } }
        public override double Y { get => base.Y; set { base.Y = value; UpdateLayout(); } }
        public override float Width { get => base.Width; set { base.Width = value; UpdateLayout(); } }
        public override float Height { get => base.Height; set { base.Height = value; UpdateLayout(); } }

        private Color color = Color.White;
        public Color Color { get => color; set { color = value; UpdateLayout(); } }

        private TextureView[] lines = new TextureView[4];

        public BorderView(Camera camera, Color? color = null, float borderWidth = 1f, double x = 0, double y = 0, float width = 100f, float height = 100f) {
            for( int i=0; i<4; i++) {
                lines[i] = new TextureView(camera, Textures.SQUARE);
                AddChild(lines[i]);
            }

            lines[0].VOrigin = VerticalOrigin.BOTTOM;
            lines[1].HOrigin = HorizontalOrigin.LEFT;
            lines[2].VOrigin = VerticalOrigin.TOP;
            lines[3].HOrigin = HorizontalOrigin.RIGHT;

            Camera = camera;
            Width = width;
            Height = height;
            X = x;
            Y = y;
            BorderWidth = borderWidth;

            if (color.HasValue) Color = color.Value;
        }





        private void UpdateLayout() {

            if( lines[0] != null) {
                foreach (var line in lines) {

                    line.Color = color;
                }

                lines[0].X = X;
                lines[0].Y = Y - Height / 2.0f;
                lines[0].Height = borderWidth;
                lines[0].Width = Width + borderWidth * 2;

                // Right border is slightly off, so we add 0.5f
                lines[1].X = X + Width / 2.0f + 0.5f;
                lines[1].Y = Y;
                lines[1].Height = Height -1f;
                lines[1].Width = borderWidth;


                lines[2].X = X;
                lines[2].Y = Y + Height / 2.0f;
                lines[2].Height = borderWidth;
                lines[2].Width = Width + borderWidth * 2;

                lines[3].X = X - Width / 2.0f;
                lines[3].Y = Y;
                lines[3].Height = Height -1f;
                lines[3].Width = borderWidth;
            }
        }

           
    }
}
