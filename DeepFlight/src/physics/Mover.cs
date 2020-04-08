
using System;

public static class Mover {


    public static void Move(Movable movable) {

        movable.VelocityX += movable.AccelerationX;
        movable.VelocityY += movable.AccelerationY;

        movable.VelocityX *= movable.Resistance;
        movable.VelocityY *= movable.Resistance;

        // Adjust to max velocity
        float totalVelocity = Math.Abs(movable.VelocityX) + Math.Abs(movable.VelocityY);
        if( totalVelocity > movable.MaxVelocity) {
            float velocityFactor = movable.MaxVelocity / totalVelocity;
            movable.VelocityX *= velocityFactor;
            movable.VelocityY *= velocityFactor;
        }

        // Update position
        movable.X += movable.VelocityX;
        movable.Y += movable.VelocityY;
    }

}