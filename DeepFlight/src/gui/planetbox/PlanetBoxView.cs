using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeepFlight.gui {
    
    /// <summary>
    /// View Component displaying the information about a Track
    /// within a "Planet"
    /// </summary>
    public class PlanetBoxView : View {

        //private Track track;
        //public Track Track { get => track; set { track = value; UpdateLayout(); } }

        private float borderScale;
        public virtual float BorderScale {
            get => borderScale;  set { borderScale = value; UpdateLayout();  }
        }

        private float size;
        public virtual float Size {
            get => size; set { size = value; UpdateLayout(); }
        }

        /// <summary>
        /// The amount the size of the view increases, when it's
        /// focused.
        /// </summary
        public virtual float FocusScale {
            get => focusScale; set { focusScale = value; UpdateLayout(); }
        }
        private float focusScale = 1.25f;
        
        /// <summary>
        /// The color of the planet, when it's focused
        /// </summary>
        public virtual Color FocusColor {
            get { return focusColor; }
            set { focusColor = value; UpdateLayout(); }
        }
        private Color focusColor = Color.White;


        /// <summary>
        /// Color of the view when it's not focused, as well
        /// as the Color of the Focus border.
        /// </summary>
        public virtual Color UnfocusColor {
            get => unfocusColor;
            set { unfocusColor = value; UpdateLayout(); }
        }
        private Color unfocusColor = Color.White;



        public override double X { get => base.X; set { base.X = value; UpdateLayout(); } }
        public override double Y { get => base.Y; set { base.Y = value; UpdateLayout(); } }


        private TextureView
            tex_Background,
            tex_FocusBorder;

        public PlanetBoxView(Camera camera, Color? focusColor = null, Color? unfocusColor = null, float size = 200f, float borderScale = 0.05f ) {
            this.size = size;
            this.borderScale = borderScale;
            

            if (focusColor.HasValue) this.focusColor = focusColor.Value;
            if (unfocusColor.HasValue) this.unfocusColor = unfocusColor.Value;

            tex_Background = new TextureView(camera, Textures.PLANET_64);
            tex_FocusBorder = new TextureView(camera, Textures.PIXEL_CIRCLE_64);
            AddChildren(tex_FocusBorder, tex_Background);

            UpdateLayout();
        }



        protected override void OnFocus() {
            // Not optimal as it updates EVERYTHING, but good enough for now
            UpdateLayout();
        }

        protected override void OnUnfocus() {
            UpdateLayout();
        }

        protected virtual void UpdateLayout() {
            // Update colors and border visibility
            tex_Background.Color = Focused ? FocusColor : UnfocusColor;
            tex_FocusBorder.Color = UnfocusColor;
            tex_FocusBorder.Hidden = !Focused;

            // Update size
            float scaledSize = (Focused) ? size * focusScale : size;
            tex_Background.Width = scaledSize;
            tex_Background.Height = scaledSize;
            tex_FocusBorder.Width = scaledSize * (1 + borderScale);
            tex_FocusBorder.Height = scaledSize * (1 + borderScale);

            // Updates position
            tex_Background.X = X;
            tex_Background.Y = Y;
            tex_FocusBorder.X = X;
            tex_FocusBorder.Y = Y;
        }

    }
}
