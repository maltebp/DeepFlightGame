

using DeepFlight.utility;
using DeepFlight.utility.KeyboardController;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System.Linq;

namespace DeepFlight.gui {
    public class TextInput : View {

        private static readonly int DEFAULT_LINE_THICKNESS = 3;
        private static readonly int DEFAULT_CURSOR_WIDTH = 2;

        public string   Input { get; set; } = "";
        public string   Dictionairy { get; set; } = Dictionary.DEFAULT;
        public int      MaxLength { get; set; } = -1;

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

            inputText = new DrawableText("", font, fontSize, color, x, y );
            cursor = new DrawableTexture(Textures.SQUARE, color, DEFAULT_CURSOR_WIDTH, (int) height, x, y );
            cursor.HOrigin = HorizontalOrigin.LEFT;
            line = new DrawableTexture(Textures.SQUARE, color, (int)width, DEFAULT_LINE_THICKNESS, x, y+height*0.95);
           
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
            System.Console.WriteLine("TextInput Key: " + e);
            if (e.Action == KeyAction.PRESSED && e.Key == Keys.Back) {
                if (Input.Length > 0) {
                    Input = Input.Substring(0, Input.Length - 1);
                }
                return true;
            }

            return false;
        }


        protected override bool OnCharInput(CharEventArgs e) {
            System.Console.WriteLine("TextInput Char: " + e);
            if (Dictionairy.Contains(e.Character)) {
                if( MaxLength < 0 || Input.Length < MaxLength) {
                    Input += e.Character;
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
            inputText.Text = Input;
            cursor.X = inputText.X + inputText.Width / 2 + 1;
        }

        protected override void OnDraw(Renderer renderer) {

            renderer.Draw(Camera, line);

            if (Input.Length > 0) {
                renderer.Draw(Camera, inputText);
            }

            if( Focused && !duringBlink) {
                renderer.Draw(Camera, cursor);
            }
            
        }
    }
}
