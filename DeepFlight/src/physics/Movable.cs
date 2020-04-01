

 
using Microsoft.Xna.Framework.Graphics;

public abstract class Movable : Collidable {

    public float VelocityX { get; set; } = 0f;
    public float VelocityY { get; set; } = 0f;
    public float AccelerationX { get; set; } = 0f;
    public float AccelerationY { get; set; } = 0f;
    public float Resistance { get; set; } = 1f; // 1 = no resistance
    public float MaxVelocity { get; set; } = 0f;

    public Movable(int width, int height) : base(width, height) { }

    public void ResetMovement() {
        VelocityX = 0;
        VelocityY = 0;
        AccelerationX = 0;
        AccelerationY = 0;
    }
}