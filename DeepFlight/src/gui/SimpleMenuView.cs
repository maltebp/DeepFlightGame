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
using static DeepFlight.src.gui.SimpleMenuView;

namespace DeepFlight.src.gui {
    class SimpleMenuView : MenuView {

        private double optionSpacing;
        private int fontSize;
        private Font font;
        private Color color;

        public delegate void OptionCallback();

        public override double X { get => base.X; set { base.X = value; UpdatePosition(); } }
        public override double Y { get => base.Y; set { base.Y = value; UpdatePosition(); } }

        public SimpleMenuView(Camera camera, Font font, int fontSize, Color color, double optionSpacing) {
            Camera = camera;
            this.font = font;
            this.fontSize = fontSize;
            this.optionSpacing = optionSpacing;
            this.color = color;

            VOrigin = VerticalOrigin.TOP;
        }

        public void AddOption(string label, OptionCallback callback) {
            AddChild(new SimpleMenuOption(this, label, callback));
            UpdatePosition();
        }

        private void UpdatePosition() {
            var y = 0.0;
            foreach (var child in Children) {
                child.X = X;
                child.Y = Y+y;
                y += child.Height + optionSpacing;
            }
        }

        // TODO: Remove this
        public void Test() {
            Console.WriteLine("Menu:  " + Hidden);
            foreach(var child in Children) {
                Console.WriteLine("Child: " + child.Hidden);

            }
        }


        class SimpleMenuOption : TextView {

            private static readonly double MARKER_MARGIN = 0.10;
            private static readonly double MARKER_SCALE = 0.25;


            public override double X { get => base.X; set { base.X = value; UpdatePosition(); } }
            public override double Y { get => base.Y; set { base.Y = value; UpdatePosition(); } }

            private OptionCallback callback { get; }
            private SelectionMarker rightMarker;
            private SelectionMarker leftMarker;

            public SimpleMenuOption(SimpleMenuView menu, string label, OptionCallback callback)
                : base(menu.Camera, label, menu.font, menu.fontSize, menu.color, 0, 0) {
                Console.WriteLine("Creating new SimpleMenuOption: " + label);
                this.callback = callback;
                rightMarker = new SelectionMarker(this);
                leftMarker = new SelectionMarker(this);
                AddChildren(rightMarker, leftMarker);
                VOrigin = VerticalOrigin.TOP;
                UpdatePosition();
            }

            protected override bool OnKeyInput(KeyEventArgs e) {
                if (e.Action == KeyAction.PRESSED && e.Key == Keys.Enter) {
                    callback();
                    return true;
                }
                return false;
            }

            protected override void OnFocus() {
                rightMarker.Hidden = false;
                leftMarker.Hidden = false;
            }

            protected override void OnUnfocus() {
                rightMarker.Hidden = true;
                leftMarker.Hidden = true;
            }

            private void UpdatePosition() {
                if( rightMarker != null && leftMarker != null) {
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
                    : base(parent.Camera, Textures.SQUARE, parent.Col, 0, 0, (float)(parent.Height * MARKER_SCALE), (float)(parent.Height * MARKER_SCALE)) {
                    Hidden = true;
                }
            }
        }
    }
}
