
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

// Holds the various textures in the game
static class Textures {

    public static Texture2D SHIP { get; private set; }
    public static Texture2D SQUARE { get; private set; }
    public static Texture2D CIRCLE { get; private set; }


    // Load the textures
    public static void LoadTextures(GraphicsDevice graphics, ContentManager content) {
        
        // SHIP
        SHIP = content.Load<Texture2D>("Content/Ship");

        // SQUARE (custom texture)
        SQUARE = new Texture2D(graphics, 10, 10);
        Color[] data = new Color[10 * 10];
        for (int i = 0; i < data.Length; ++i)
            data[i] = new Color(255, 255, 255);
        SQUARE.SetData(data);

        // CIRCLE
        CIRCLE = content.Load<Texture2D>("Content/CircleTexture");
    }
}