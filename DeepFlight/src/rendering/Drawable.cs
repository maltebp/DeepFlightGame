
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

public abstract class Drawable : Entity {

    public Texture2D Texture { get; private set; }
    public Color Col { get; set; } = Color.White;

    protected Drawable(Texture2D texture, int width, int height) : base(width, height) {
        Texture = texture;
    }


}