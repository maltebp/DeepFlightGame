﻿using DeepFlight.utility;
using DeepFlight.utility.KeyboardController;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Linq;

namespace DeepFlight.gui {


    /// <summary>
    /// Generic View which handles a single line of text input.
    /// The View draws the input text, a bottom line, which thickens
    /// when the view is focused, as well as a text carret which blinks
    /// where the next character will be inserted
    /// </summary>
    public class TextInputView : View {

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
        private TextureView caret;
        private TextureView line;

        private double blinkDuration = 0.75;
        private double blinkCooldown;
        private bool duringBlink = false;


        public TextInputView(Camera camera, string label, Font font, int fontSize, Color color, double x, double y, float width) : base(camera) {
            X = x;
            Y = y;
            Width = (int) width;

            // Input Text field
            inputText = new TextView(Camera, "", font, fontSize, color, x, y);
            inputText.VOrigin = VerticalOrigin.BOTTOM;
            AddChild(inputText);

            // Cursor
            caret = new TextureView(Camera, Textures.SQUARE, color, x, inputText.GetCenterY() - 8, DEFAULT_CURSOR_WIDTH, inputText.Height*0.75f );
            caret.HOrigin = HorizontalOrigin.LEFT;

            // Fancy line below input
            line = new TextureView(Camera, Textures.SQUARE, color, x, y - LINE_MARGIN, width, LINE_THICKNESS);

            // Input Label displaying what to input
            Label = new TextView(Camera, label, font, fontSize * 0.6, color, x- width / 2, y + LINE_MARGIN * 2 + LINE_THICKNESS);
            Label.VOrigin = VerticalOrigin.TOP;
            Label.HOrigin = HorizontalOrigin.LEFT;
            AddChild(Label);
                                            
            blinkCooldown = blinkDuration;
        }

        // Restart carret blinking and thicken the bottom line
        protected override void OnFocus() {
            duringBlink = false;
            blinkCooldown = 1;
            line.Height *= 2f;
        }

        // Turn off blinking, hide carret and makes the line thin again
        protected override void OnUnfocus() {
            line.Height *= 0.5f;
        }


        // Handle backspace and enter
        protected override bool OnKeyInput(KeyEventArgs e) {
            if (e.Action == KeyAction.PRESSED && e.Key == Keys.Back) {
                if (Text.Length > 0) {
                    Text = Text.Substring(0, Text.Length - 1);
                }
                return true;
            }

            return false;
        }


        // Consume character event, and insert it into the text view
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
            caret.X = inputText.GetCenterX() + inputText.Width / 2+ 1;
        }


        protected override void OnDraw(Renderer renderer) {
            renderer.DrawTexture(Camera, line);
            renderer.DrawText(Camera, Label);

            if (Text.Length > 0) {
                renderer.DrawText(Camera, inputText);
            }

            if( Focused && !duringBlink) {
                renderer.DrawTexture(Camera, caret);
            }  
        }
    }
}
