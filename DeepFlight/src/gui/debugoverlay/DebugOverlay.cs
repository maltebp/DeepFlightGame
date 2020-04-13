using DeepFlight.gui;
using DeepFlight.rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeepFlight.src.gui.debugoverlay {
    public class DebugOverlay : View {

        private TextInput inputField;

        public DebugInfoView Info { get; }

        public DebugOverlay() : base(null) {

            // Create private Camera with center in top right corner
            Camera = new Camera( 
                x: ScreenController.BaseWidth / 2.0, 
                y: ScreenController.BaseHeight / 2.0, 
                layer: 0.1f);

            // Info View
            Info = new DebugInfoView(Camera);
            Info.Height = 250;
            Info.Width = 300;
            AddChild(Info);
        }
    }
}
