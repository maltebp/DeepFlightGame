


using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

class Settings {

    // Kill Switch terminates the program immediately
    private static readonly bool KILL_SWITCH_ENABLED = true;
    private static readonly Keys KILL_SWITCH_KEY = Keys.Escape;

    private static readonly bool SAVE_TRACK = true;
    private static readonly bool LOAD_TRACK = true;
    private static readonly bool RANDOM_TRACK = true;
    private static readonly int TRACK_SEED = 2000; // Seed to use if RANDOM_TRACK is false

    // Zoom Settings    
    private static readonly float ZOOM_DEFAULT = 8f;
    private static readonly float ZOOM_MAX = 30f;
    private static readonly float ZOOM_MIN = 0.5f;
    private static readonly float ZOOM_FACTOR = 0.001f; // How "fast" to zoom

    public static readonly float GAME_CHECKPOINT_SIZE = 20f;

    public static readonly Color CLEAR_COLOR = Color.Green;

    public static readonly Color COLOR_PRIMARY = new Color(212, 69, 24);

}