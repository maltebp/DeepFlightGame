using Microsoft.Xna.Framework;
using System.Collections.Generic;
using static DeepFlight.src.gui.debugoverlay.InfoLine;


namespace DeepFlight.src.gui.debugoverlay {
    public class DebugInfoView : View {

        private int fontSize = 14;
        public int FontSize { get => fontSize; set { fontSize = value; UpdateLayout(); }}

        private Font font = Fonts.DEFAULT;
        public Font Font { get => font; set { font = value; UpdateLayout(); }}

        // Line spacing is in pixels
        private int lineSpacing = 8;
        public int LineSpacing { get => lineSpacing; set { lineSpacing = value; UpdateLayout(); } }

        // Distance from the edge of the background to the content (info lines)
        private int padding = 5;
        public int Padding { get => padding; set { padding = value; UpdateLayout(); } }

        public override double X { get => base.X; set { base.X = value; UpdateLayout(); } }
        public override double Y { get => base.Y; set { base.Y = value; UpdateLayout(); } }

        public override float  Width { get => base.Width; set { base.Width = value; UpdateLayout(); } }
        public override float  Height { get => base.Height; set { base.Height = value; UpdateLayout(); } }

        private LinkedList<InfoLine> infoLines = new LinkedList<InfoLine>();


        public DebugInfoView(Camera camera) : base(camera) {
            VOrigin = VerticalOrigin.TOP;
            HOrigin = HorizontalOrigin.LEFT;
            BackgroundColor = new Color(Color.Black, 100);
        }

        public InfoLine AddInfoLine(string tag, string initialInfo, OnInfoLineUpdate updateCallback) {
            TextView textView = new TextView(Camera, "", font, fontSize, Color.White, 0, 0);
            textView.VOrigin = VerticalOrigin.TOP;
            textView.HOrigin = HorizontalOrigin.LEFT;
            InfoLine infoLine = new InfoLine(textView, tag, initialInfo, updateCallback);
            infoLines.AddLast(infoLine);
            AddChild(infoLine.TextView);
            UpdateLayout();
            return infoLine;
        }


        public void RemoveInfoLine(InfoLine lineToRemove) {
            RemoveChild(lineToRemove.TextView);
            infoLines.Remove(lineToRemove);
        }

        protected override void OnUpdate(double deltaTime) {
            foreach (var infoLine in infoLines) infoLine.UpdateInfo();
        }
   

        /// <summary>
        /// Updates the layout information of all InfoLines and the
        /// background, according to the current visual state of
        /// the DebugInfoView.
        /// </summary>
        private void UpdateLayout() {


            // Update each info line
            double contentX = X + Padding;
            double contentY = Y + Padding;

            double maxWidth = 0;
            foreach (var infoLine in infoLines) {
                var textView = infoLine.TextView;
                textView.Font = font;
                textView.Size = fontSize;
                textView.X = contentX;
                textView.Y = contentY;
                contentY += textView.Height + lineSpacing;
                if (textView.Width > maxWidth) maxWidth = textView.Width;
            }          
        }
    }


    public class InfoLine {
        public string Tag { get; set; }
        public string Info { get; set; }

        public TextView TextView { get; }

        private OnInfoLineUpdate updateCallback;

        public InfoLine(TextView textView, string tag, string initialInfo, OnInfoLineUpdate updateCallback) {
            this.updateCallback = updateCallback;
            Tag = tag;
            Info = initialInfo;
            TextView = textView;
            TextView.Text = Tag + ": " + Info;
        }

        public void UpdateInfo () {
            updateCallback(this);
            TextView.Text = Tag + ": " + Info;
        }

        public delegate void OnInfoLineUpdate(InfoLine currentInfo);
    }
}
