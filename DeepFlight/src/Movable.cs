


using Microsoft.Xna.Framework.Graphics;

class Movable : Drawable {

    public float VelocityX { get; set; } = 0f;
    public float VelocityY { get; set; } = 0f;
    public float AccelerationX { get; set; } = 0f;
    public float AccelerationY { get; set; } = 0f;
    public float Resistance { get; set; } = 1f; // 1 = no resistance
    public float MaxVelocity { get; set; }

    public Movable(Texture2D texture, int width, int height, float maxVelocity) : base(texture, width, height) {
        MaxVelocity = maxVelocity;
    }
}