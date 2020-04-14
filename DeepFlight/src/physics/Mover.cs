
using System;

public static class Mover {


    public static void Move(Movable movable, double deltaTime) {

        movable.VelocityX += (movable.AccelerationX - movable.Resistance*movable.VelocityX)*(float)deltaTime;
        movable.VelocityY += (movable.AccelerationY - movable.Resistance*movable.VelocityY)*(float)deltaTime;

        movable.Rotation += (float) (movable.RotationVelocity * deltaTime);

        // Update position
        movable.X += movable.VelocityX*deltaTime;
        movable.Y += movable.VelocityY * deltaTime; ;
    }

}