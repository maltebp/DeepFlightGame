
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

// Holds the various textures in the game
static class Textures {

    public static Texture2D SHIP { get; private set; }
    public static Texture2D SQUARE { get; private set; }
    public static Texture2D CIRCLE { get; private set; }
    public static Texture2D TITLE { get; private set; }
    public static Texture2D PIXEL_CIRCLE_9 { get; private set; }
    public static Texture2D PIXEL_CIRCLE_16 { get; private set; }
    public static Texture2D PIXEL_CIRCLE_64 { get; private set; }
    public static Texture2D PLANET_64 { get; private set; }
    public static Texture2D BACKGROUND { get; private set; }


    private static GraphicsDevice graphics;

    // Load the textures
    public static void LoadTextures(GraphicsDevice graphics, ContentManager content) {
        Textures.graphics = graphics;

        // SHIP
        SHIP = content.Load<Texture2D>("Content/Images/Ship");

        TITLE = content.Load<Texture2D>("Content/Images/GameTitle");

        // SQUARE (custom texture)
        SQUARE = new Texture2D(graphics, 10, 10);
        Color[] data = new Color[10 * 10];
        for (int i = 0; i < data.Length; ++i)
            data[i] = new Color(255, 255, 255);
        SQUARE.SetData(data);

        // CIRCLE
        CIRCLE = content.Load<Texture2D>("Content/Shapes/CircleTexture");

        BACKGROUND = content.Load<Texture2D>("Content/Images/Background");
        
        PIXEL_CIRCLE_9 = content.Load<Texture2D>("Content/Shapes/PixelCircle9");
        PIXEL_CIRCLE_16 = content.Load<Texture2D>("Content/Shapes/PixelCircle_16");
        PIXEL_CIRCLE_64 = content.Load<Texture2D>("Content/Shapes/PixelCircle_64");

        PLANET_64 = content.Load<Texture2D>("Content/Images/Planet_64");
    }

    public static Texture2D CreateTexture(int width, int height) {
        return new Texture2D(graphics, width, height);
    }
}