using Microsoft.Xna.Framework;
using System.Collections.Generic;
using static DeepFlight.src.gui.debugoverlay.DebugInfoLine;


namespace DeepFlight.src.gui.debugoverlay {

    /// <summary>
    /// View which displays a series of InfoLines on a semi-transparent background.
    ///
    /// The InfoLines are lines of user defined information, and can be added
    /// or removed dynamically.
    /// </summary>
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

        private LinkedList<DebugInfoLine> infoLines = new LinkedList<DebugInfoLine>();


        public DebugInfoView(Camera camera) : base(camera) {
            VOrigin = VerticalOrigin.TOP;
            HOrigin = HorizontalOrigin.LEFT;
            BackgroundColor = new Color(Color.Black, 100);
        }

        /// <summary>
        /// Add a new InfoLine to the InfoView, which is displayed in the manner:
        /// 
        ///     [TAG]: [INFORMATION]
        /// 
        /// The line needs to be manually removed again using the RemoveInfoLine(..) method
        /// </summary>
        /// <param name="tag">A description of the information, displayed in the line</param>
        /// <param name="initialInfo">The initial information, before an update has been called</param>
        /// <param name="updateCallback">The delegate which updates the information of the line. This is called frequently.</param>
        /// <returns></returns>
        public DebugInfoLine AddInfoLine(string tag, string initialInfo, OnInfoLineUpdate updateCallback) {
            TextView textView = new TextView(Camera, "", font, fontSize, Color.White, 0, 0);
            textView.VOrigin = VerticalOrigin.TOP;
            textView.HOrigin = HorizontalOrigin.LEFT;
            DebugInfoLine infoLine = new DebugInfoLine(textView, tag, initialInfo, updateCallback);
            infoLines.AddLast(infoLine);
            AddChild(infoLine.TextView);
            UpdateLayout();
            return infoLine;
        }


        public void RemoveInfoLine(DebugInfoLine lineToRemove) {
            RemoveChild(lineToRemove.TextView);
            lineToRemove.TextView.Terminate();
            infoLines.Remove(lineToRemove);
        }

        protected override void OnUpdate(double deltaTime) {
            foreach (var infoLine in infoLines) infoLine.UpdateInfo();
            UpdateLayout();
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

            // Update size to InfoLines
            Width =(float) (maxWidth + padding * 2);
            Height = (float) (contentY - Y + padding * 2);
        }
    }


    public class DebugInfoLine {
        public string Tag { get; set; }
        public string Info { get; set; }

        public TextView TextView { get; }

        private OnInfoLineUpdate updateCallback;

        public DebugInfoLine(TextView textView, string tag, string initialInfo, OnInfoLineUpdate updateCallback) {
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

        public delegate void OnInfoLineUpdate(DebugInfoLine currentInfo);
    }
}
