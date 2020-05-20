using DeepFlight.gui;
using DeepFlight.rendering;
using DeepFlight.utility.KeyboardController;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static DeepFlight.src.gui.SimpleMenuOption;
using static DeepFlight.src.gui.SimpleMenuView;

namespace DeepFlight.src.gui {


    /// <summary>
    /// The simple menu view, allows for easy creation of simple text menus
    /// (a menu option is just a text).
    /// </summary>
    public class SimpleMenuView : MenuView {

        public double OptionSpacing { get; }
        public int FontSize { get; }
        public Font Font { get; }
        public Color Color { get; }

        public override double X { get => base.X; set { base.X = value; UpdatePosition(); } }
        public override double Y { get => base.Y; set { base.Y = value; UpdatePosition(); } }

        public SimpleMenuView(Camera camera, Font font, int fontSize, Color color, double optionSpacing) {
            Camera = camera;
            Font = font;
            FontSize = fontSize;
            OptionSpacing = optionSpacing;
            Color = color;

            VOrigin = VerticalOrigin.TOP;
        }

        public void AddSimpleOption(string label, OptionSelectCallback callback) {
            AddMenuOption(new SimpleMenuOption(this, label), callback);
            UpdatePosition();
        }

        private void UpdatePosition() {
            var y = 0.0;
            foreach (var child in Children) {
                child.X = X;
                child.Y = Y+y;
                y += child.Height + OptionSpacing;
            }
        }
        
    }


    /// <summary>
    /// Simple TextView which displays pair of squares when focused (i.e. by MenuView),
    /// and contains a callback which is triggered when Enter is pressed while focused.
    /// </summary>
    public class SimpleMenuOption : TextView {

        private static readonly double MARKER_MARGIN = 0.10;
        private static readonly double MARKER_SCALE = 0.25;

        public override double X { get => base.X; set { base.X = value; UpdatePosition(); } }
        public override double Y { get => base.Y; set { base.Y = value; UpdatePosition(); } }

        private SelectionMarker rightMarker;
        private SelectionMarker leftMarker;

        public SimpleMenuOption(Camera camera, string label, Font font, int fontSize, Color color)
            : base(camera, label, font, fontSize, color, 0, 0) {
            rightMarker = new SelectionMarker(this);
            rightMarker.Hidden = true;
            leftMarker = new SelectionMarker(this);
            leftMarker.Hidden = true;
            AddChildren(rightMarker, leftMarker);
            VOrigin = VerticalOrigin.TOP;
            UpdatePosition();
        }


        /// <summary>
        /// Constructs SimpleMenuOption from the layout of a SimpleMenuView
        /// </summary>
        public SimpleMenuOption(SimpleMenuView menu, string label)
            : this(menu.Camera, label, menu.Font, menu.FontSize, menu.Color) { }

        protected override void OnFocus() {
            rightMarker.Hidden = false;
            leftMarker.Hidden = false;
        }

        protected override void OnUnfocus() {
            rightMarker.Hidden = true;
            leftMarker.Hidden = true;
        }

        private void UpdatePosition() {
            if (rightMarker != null && leftMarker != null) {
                rightMarker.Y = GetCenterY();
                leftMarker.Y = GetCenterY();

                double centerDistance = Width / 2 + Width * MARKER_MARGIN;
                rightMarker.X = GetCenterX() + centerDistance;
                leftMarker.X = GetCenterX() - centerDistance;
            }
        }

        // Square appearing to the left and right of the selected option
        private class SelectionMarker : TextureView {
            public SelectionMarker(TextView parent)
                : base(parent.Camera, Textures.SQUARE, parent.Color, 0, 0, (float)(parent.Height * MARKER_SCALE), (float)(parent.Height * MARKER_SCALE)) {
                Hidden = true;
            }
        }
    }
}
