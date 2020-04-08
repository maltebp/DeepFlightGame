

using DeepFlight.utility;
using DeepFlight.utility.KeyboardController;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Linq;

namespace DeepFlight.gui {
    public class TextInput : View {

        private static readonly int LINE_THICKNESS = 3;
        private static readonly int LINE_MARGIN = 3;
        private static readonly int DEFAULT_CURSOR_WIDTH = 3;

        public string   Text { get; set; } = "";
        public string   Dictionairy { get; set; } = CharLists.DEFAULT;
        public int      MaxLength { get; set; } = -1;
        public bool     PasswordInput { get; set; }
        public override float Height {
            get => throw new Exception("Height is not implemented in TextInput");
        }
        
        public TextView Label { get; }

        private TextView inputText;
        private TextureView cursor;
        private TextureView line;

        private double blinkDuration = 0.75;
        private double blinkCooldown;
        private bool duringBlink = false;


        private int cursorPosition = 0;

        public TextInput(Camera camera, string label, Font font, int fontSize, Color color, double x, double y, float width) : base(camera) {
            X = x;
            Y = y;
            Width = (int) width;

            // Input Text field
            inputText = new TextView(Camera, "", font, fontSize, color, x, y);
            inputText.VOrigin = VerticalOrigin.BOTTOM;
            AddChild(inputText);

            // Cursor
            cursor = new TextureView(Camera, Textures.SQUARE, color, x, inputText.GetCenterY() - 8, DEFAULT_CURSOR_WIDTH, inputText.Height*0.75f );
            cursor.HOrigin = HorizontalOrigin.LEFT;

            line = new TextureView(Camera, Textures.SQUARE, color, x, y - LINE_MARGIN, width, LINE_THICKNESS);

            Label = new TextView(Camera, label, font, fontSize * 0.6, color, x- width / 2, y + LINE_MARGIN * 2 + LINE_THICKNESS);
            Label.VOrigin = VerticalOrigin.TOP;
            Label.HOrigin = HorizontalOrigin.LEFT;
            AddChild(Label);
                                            
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
            renderer.Draw(Camera, Label);

            if (Text.Length > 0) {
                renderer.Draw(Camera, inputText);
            }

            if( Focused && !duringBlink) {
                renderer.Draw(Camera, cursor);
            }  
        }
    }
}
