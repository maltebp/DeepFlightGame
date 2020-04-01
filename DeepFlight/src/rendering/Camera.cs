


using System;

public class Camera {

    public double X { get; set; } = 0;
    public double Y { get; set; } = 0;

    private float rotation = 0f; // Radians
    public float Rotation {
        get => rotation;
        set => rotation = MathExtension.Mod(value, (float)(2 * Math.PI));
    }

    public float Zoom { get; set; } = 1f;

    public Entity Transform(Entity drawable, int screenWidth, int screenHeight) {
        Entity trans = new Entity();

        // Scale the object to camera
       
        trans.X = drawable.X * Zoom;
        trans.Y = drawable.Y * Zoom;

        // Adjust to camera position
        trans.X += (screenWidth / 2 - X * Zoom);
        trans.Y += (screenHeight / 2 - Y * Zoom);

        // Adjust to center of entity
        trans.X -= drawable.Width / 2;
        trans.Y -= drawable.Height / 2;
        double centerX = screenWidth / 2;
        double centerY = screenHeight / 2;

        // Rotate around camera center
        trans.X = Math.Cos(-Rotation) * (trans.X - centerX) - Math.Sin(-Rotation) * (trans.Y - centerY) + centerX;
        trans.Y = Math.Sin(-Rotation) * (trans.X - centerX) + Math.Cos(-Rotation) * (trans.Y - centerY) + centerY;

        // Scale width and height
        trans.Width = drawable.Width * Zoom * drawable.scale;
        trans.Height = drawable.Height * Zoom * drawable.scale;

        trans.Rotation = drawable.Rotation - Rotation;

        return trans;
    }

}