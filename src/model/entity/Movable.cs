

 
using Microsoft.Xna.Framework.Graphics;
using System;

public abstract class Movable : Collidable {

    public float VelocityX { get; set; } = 0f;
    public float VelocityY { get; set; } = 0f;
    public float AccelerationX { get; set; } = 0f;
    public float AccelerationY { get; set; } = 0f;
    public float Resistance { get; set; } = 1f; // 1 = no resistance
    public float MaxVelocity { get; set; } = 0f;
    public float RotationVelocity = 0f;

    public float Velocity { get => Math.Abs(VelocityX) + Math.Abs(VelocityY); }

    public Movable() : base(0,0) { }

    public Movable(float width, float height) : base(width, height) { }

    /// <summary>
    /// Resets the movables movements, by setting all acceleration and velocity to 0
    /// </summary>
    public void ResetMovement() {
        VelocityX = 0;
        VelocityY = 0;
        AccelerationX = 0;
        AccelerationY = 0;
    }

    /// <summary>
    /// Updates the movable by moving, accelerate and rotate it
    /// </summary>
    /// <param name="deltaTime"> Delta time to determine how much to update it </param>
    public void UpdateMovement(double deltaTime) {
        VelocityX += (AccelerationX - Resistance* VelocityX)*(float) deltaTime;
        VelocityY += (AccelerationY - Resistance* VelocityY)*(float) deltaTime;

        Rotation += (float) (RotationVelocity* deltaTime);

        // Update position
        X += VelocityX* deltaTime;
        Y += VelocityY* deltaTime;
    }
}