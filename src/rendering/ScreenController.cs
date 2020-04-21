using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeepFlight.rendering {

    /// <summary>
    /// Handles information of the screen, including logical base dimensions, and
    /// resolutions.
    /// </summary>
    public static class ScreenController {
        private static readonly double  BASE_HEIGHT = 720;
        private static readonly bool    FULLSCREEN  = false;

        // Logical width/height of the screen (used as a base
        // for scaling different resolutions)
        public static double BaseWidth { get; private set; }
        public static double BaseHeight { get; private set; } = BASE_HEIGHT;

        public static double ScreenScale { get; private set; }
        public static Resolution Resolution { get; private set; }
        public static Resolution ScreenSize { get; private set; }

        // Supported resolutions in th esystem
        public static Resolution[] allResolutions = {
            new Resolution(1920, 1080, 16, 9),
            new Resolution(1366, 768, 16, 9),
            new Resolution(1280, 720, 16, 9),
            new Resolution(1440, 900, 16, 10) // Mac
        };

        public static Resolution[] availableResolutions = null;

        private static GraphicsDeviceManager graphics;

        private static bool initialized = false;
        public static bool Initialized { get => initialized;  }

        public static void Initialize(GraphicsDeviceManager graphics) {
            ScreenController.graphics = graphics;
            double screenWidth = graphics.GraphicsDevice.DisplayMode.Width;
            double screenHeight = graphics.GraphicsDevice.DisplayMode.Height;

            bool foundRes = false;
            foreach(var res in allResolutions) {
                if( res.Width == screenWidth && res.Height == screenHeight) {
                    foundRes = true;
                    ScreenSize = res;
                    break;
                }
            }

            if (!foundRes)
                throw new NullReferenceException(string.Format("Screen resolution {0}x{1} not supported.", screenWidth, screenHeight) );

            // Adjust available resolutions:
            var availableResolutionsLinked = new LinkedList<Resolution>();
            foreach(var res in allResolutions) {
                if (ScreenSize.WidthRatio == res.WidthRatio && ScreenSize.HeightRatio == res.HeightRatio)
                    availableResolutionsLinked.AddLast(res);
            }
            availableResolutions = availableResolutionsLinked.ToArray();

            // Get the actual screen dimensions
            if (FULLSCREEN)
                graphics.ToggleFullScreen();

            SetResolution(ScreenSize);

            // Check screen size
            initialized = true;

            Console.WriteLine("\nInitialized ScreenManager!");
            Console.WriteLine("Screen size: {0}", ScreenSize);
            Console.Write("Available resolutions: ");
            foreach( var res in availableResolutions )
                Console.Write("{0}  ", res.DimensionString());
            Console.WriteLine("\n");
        }

        public static void SetResolution(Resolution resolution) {
            foreach (var res in availableResolutions) {
                if (resolution == res) {
                    Resolution = res;
                    graphics.PreferredBackBufferWidth = resolution.Width;
                    graphics.PreferredBackBufferHeight = resolution.Height;
                    graphics.ApplyChanges();

                    ScreenScale = graphics.PreferredBackBufferHeight / (float) BaseHeight;
                    BaseWidth = BaseHeight * resolution.Width / (double) resolution.Height;

                    Console.WriteLine("Set resolution to: {0} (scale: {1})", Resolution, ScreenScale);
                    return;
                }
            }
            throw new Exception(string.Format("Screen resolution '{0}' not found", resolution));
        }
    
    }

    public struct Resolution {
        public int Width { get; }
        public int Height { get; }
        public int WidthRatio { get; }
        public int HeightRatio { get; }

        public Resolution(int width, int height, int widthRatio, int heightRatio) {
            Width = width;
            Height = height;
            WidthRatio = widthRatio;
            HeightRatio = heightRatio;
        }

        public override string ToString() {
            return string.Format("Resolution( " +
                "width={0}, " +
                "height={1}, " +
                "dim.={2}x{3} " +
                ")", Width, Height, WidthRatio, HeightRatio);
        }

        public string DimensionString() {
            return Width + "x" + Height;
        }

        public string RatioString() {
            return WidthRatio + "x" + HeightRatio;
        }

        public override bool Equals(object other) {
            if (other == null) return false;
            if (!(other is Resolution)) return false;
            Resolution otherRes = (Resolution)other;
            return Width == otherRes.Width &&
                    Height == otherRes.Height &&
                    WidthRatio == otherRes.WidthRatio &&
                    HeightRatio == otherRes.HeightRatio;
        }

        public static bool operator ==(Resolution r1, Resolution r2) {
            return r1.Equals(r2);
        }

        public static bool operator !=(Resolution r1, Resolution r2) {
            return !r1.Equals(r2);
        }

    }
}
