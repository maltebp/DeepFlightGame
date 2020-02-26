
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

static class Textures {

    public static Texture2D SHIP { get; private set; }

    public static void LoadTextures(ContentManager content) {
        SHIP = content.Load<Texture2D>("Content/Ship");
    }

}