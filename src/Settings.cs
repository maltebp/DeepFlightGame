
using System;
using Microsoft.Xna.Framework;

public class Settings {


    public static float SHIP_RESISTANCE = 0.90f;
    public static float SHIP_ACCELERATION = 150f;
    public static float SHIP_ROTATION_VELOCITY = (float) Math.PI;

    public static readonly string OFFLINE_TRACK_TIMES_FILENAME = "tracktimes.bin";
        
    public static readonly string OFFLINE_TRACKS_FOLDER = "offlinetracks";

    public static readonly string WEBSITE_URL = "https://master.d3lj15etjpqs5m.amplifyapp.com/#/"; //"http://www.maltebp.dk";

    public static readonly float GAME_CHECKPOINT_SIZE = 30f;

    public static readonly Color CLEAR_COLOR = Color.Green;

    public static readonly float TRACK_COLOR_ADJUST_TRACK   = 0.75f;
    public static readonly float TRACK_COLOR_ADJUST_WALL    = 1.25f;

    public static readonly Color COLOR_PRIMARY = new Color(40, 44, 52);

}