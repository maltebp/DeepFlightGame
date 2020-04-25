using DeepFlight.gui;
using DeepFlight.rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeepFlight.src.gui.debugoverlay {

    /// <summary>
    /// Container class for the entire Debug view, which allows for
    /// expanding the view without having to changet the functionality of
    /// displaying it.
    /// 
    /// </summary>
    public class DebugOverlay : View {

        // Class is a Singleton, and the instance may be fetched
        // using this
        private static DebugOverlay instance;
        public static DebugOverlay Instance {
            get{
                if (instance == null)
                    instance = new DebugOverlay();
                return instance;
            } 
        }

        // Property allowing for ease of access of the InfoView
        // on the singleton instance.
        private DebugInfoView info;
        public static DebugInfoView Info { 
            get {
                return Instance.info;
            }
        }

        private DebugOverlay() : base(null) {
            if (!ScreenController.Initialized)
                throw new InvalidOperationException("ScreenController must be initialized before getting the DebugOverlay instance");

            // Create private Camera with center in top right corner
            Camera = new Camera( 
                x: ScreenController.BaseWidth / 2.0, 
                y: ScreenController.BaseHeight / 2.0, 
                layer: 0.1f);

            // Info View
            info = new DebugInfoView(Camera);
            AddChild(info);
        }
        
    }
}
