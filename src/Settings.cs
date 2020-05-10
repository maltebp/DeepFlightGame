
using System;
using Microsoft.Xna.Framework;

/// <summary>
/// Static class containing Settings as global variables
/// </summary>
public static class Settings {

    // Visuals
    public static readonly Color COLOR_PRIMARY  = new Color(40, 44, 52);
    public static readonly Color CLEAR_COLOR    = Color.Green;
    public static readonly Color SHIP_COLOR = new Color(50, 50, 50);

    // Track Rendering
    public static readonly int TRACK_RENDER_DISTANCE    = 200;  // Minimum number of blocks to render in each direction
    public static readonly int TRACK_CHUNK_SIZE         = 50;   // Number of blocks in each chunk
    public static readonly int TRACK_COLOR_ADJUST_TRACK = 50;
    public static readonly int TRACK_COLOR_ADJUST_WALL = 40;

    // Game Logic 
    public static float SHIP_ACCELERATION       = 180f;
    public static float SHIP_RESISTANCE         = 0.94f;
    public static float SHIP_ROTATION_VELOCITY  = (float) Math.PI; // Radians per second
    public static float CHECKPOINT_SIZE = 30f;  // Diameter of checkpoints

    // Files and folders
    public static readonly string OFFLINE_TRACK_TIMES_FILENAME = "tracktimes.bin";
    public static readonly string OFFLINE_TRACKS_FOLDER = "offlinetracks";
    public static readonly string WEBSITE_URL = "http://www.maltebp.dk";
    public static readonly string GAME_API_URL = "http://maltebp.dk:10000/gameapi";
}