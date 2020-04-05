

 
using Microsoft.Xna.Framework.Graphics;

public abstract class Movable : Collidable {

    public float VelocityX { get; set; } = 0f;
    public float VelocityY { get; set; } = 0f;
    public float AccelerationX { get; set; } = 0f;
    public float AccelerationY { get; set; } = 0f;
    public float Resistance { get; set; } = 1f; // 1 = no resistance
    public float MaxVelocity { get; set; } = 0f;

    public Movable() : base(0,0) { }

    public Movable(int width, int height) : base(width, height) { }


    /// <summary>
    /// Deep copy constructor
    /// WARNING: Colliders are NOT copied!
    /// </summary>
    public Movable(Movable original) : base(original) {
        VelocityX = original.VelocityX;
        VelocityY = original.VelocityY;
        AccelerationX = original.AccelerationX;
        AccelerationY = original.AccelerationY;
        Resistance = original.Resistance;
        MaxVelocity = original.MaxVelocity;
    }

    public void ResetMovement() {
        VelocityX = 0;
        VelocityY = 0;
        AccelerationX = 0;
        AccelerationY = 0;
    }
}