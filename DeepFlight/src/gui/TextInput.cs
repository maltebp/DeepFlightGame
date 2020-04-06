

using DeepFlight.utility;
using DeepFlight.utility.KeyboardController;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Linq;

namespace DeepFlight.gui {
    public class TextInput : View {

        private static readonly int DEFAULT_LINE_THICKNESS = 3;
        private static readonly int DEFAULT_CURSOR_WIDTH = 2;

        public string   Text { get; set; } = "";
        public string   Dictionairy { get; set; } = CharLists.DEFAULT;
        public int      MaxLength { get; set; } = -1;
        public bool     PasswordInput { get; set; }

        private DrawableText inputText;
        private DrawableTexture cursor;
        private DrawableTexture line;


        private double blinkDuration = 0.75;
        private double blinkCooldown;
        private bool duringBlink = false;


        private int cursorPosition = 0;

        public TextInput(Camera camera, Font font, int fontSize, Color color, double x, double y, double width, double height) : base(camera) {
            X = x;
            Y = y;
            Width = (int) width;
            Height = (int) height;

            var bottomY = GetCenterY() + height / 2;

            // Input Text field
            inputText = new DrawableText("", font, fontSize, color, x, bottomY + height*0.1 );
            inputText.VOrigin = VerticalOrigin.BOTTOM;

            // Cursor
            cursor = new DrawableTexture(Textures.SQUARE, color, DEFAULT_CURSOR_WIDTH, (int) (height*1), x, y );
            cursor.HOrigin = HorizontalOrigin.LEFT;
            cursor.VOrigin = VerticalOrigin.BOTTOM;

            line = new DrawableTexture(Textures.SQUARE, color, (int)width, DEFAULT_LINE_THICKNESS, x, bottomY);
            line.VOrigin = VerticalOrigin.BOTTOM;

           
            blinkCooldown = blinkDuration;
        }




        protected override void OnFocus() {
            duringBlink = false;
            blinkCooldown = 1;
            line.Height *= 2f;
        }

        protected override void OnUnfocus() {
            line.Height *= 0.5f;
        }

        /// <summary>
        /// Returns true
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        protected override bool OnKeyInput(KeyEventArgs e) {
            if (e.Action == KeyAction.PRESSED && e.Key == Keys.Back) {
                if (Text.Length > 0) {
                    Text = Text.Substring(0, Text.Length - 1);
                }
                return true;
            }

            return false;
        }


        protected override bool OnCharInput(CharEventArgs e) {
            if (Dictionairy.Contains(e.Character)) {
                if( MaxLength < 0 || Text.Length < MaxLength) {
                    Text += e.Character;
                }
                return true;
            }
            return false;
        }

        protected override void OnUpdate(double timeDelta) {
            blinkCooldown -= timeDelta;
            if (blinkCooldown < 0) {
                duringBlink = !duringBlink;
                blinkCooldown = blinkDuration;
            }
            if (PasswordInput)
                inputText.Text = new string('*', Text.Length);
            else
                inputText.Text = Text;
            cursor.X = inputText.GetCenterX() + inputText.Width / 2+ 1;
        }

        protected override void OnDraw(Renderer renderer) {

            renderer.Draw(Camera, line);

            if (Text.Length > 0) {
                renderer.Draw(Camera, inputText);
            }

            if( Focused && !duringBlink) {
                renderer.Draw(Camera, cursor);
            }
            
        }
    }
}
