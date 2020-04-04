


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

    /// <summary>
    /// Transform the given Entity to the camera view, by
    /// scaling, moving and rotation the entity
    /// </summary>
    /// <param name="transformed">The Entity object to be transformed</param>
    /// <param name="rotateObject">Whether or not to rotate the object with the camera
    /// (i.e. it might be useful to not rotate text)</param>
    public void Transform(Entity transformed, bool rotateObject) {

        ////float scaleVal = graphics.PreferredBackBufferHeight / (float)LOGICAL_SCREEN_HEIGHT
        //transformed.X = entity.X;
        //transformed.Y = entity.Y;
        //transformed.Width = entity.Width;
        //transformed.Height = entity.Height;

        //// Adjust to camera view
        transformed.X -= X;
        transformed.Y -= Y;

        //// Apply camera zoom
        transformed.X *= Zoom;
        transformed.Y *= Zoom;
        transformed.scale *= Zoom;
        //transformed.Width *= Zoom;
        //transformed.Height *= Zoom;

        //// Rotate position around cameras center (which is now 0,0)
        double centerX = 0;
        double centerY = 0;

        //p'x = cos(theta) * (px-ox) - sin(theta) * (py-oy) + ox
        double x = Math.Cos(-Rotation) * (transformed.X - centerX) - Math.Sin(-Rotation) * (transformed.Y - centerY) + centerX;
        double y = Math.Sin(-Rotation) * (transformed.X - centerX) + Math.Cos(-Rotation) * (transformed.Y - centerY) + centerY;

        transformed.X = x;
        transformed.Y = y;

        //p'y = sin(theta) * (px-ox) + cos(theta) * (py-oy) + oy

        // Adjust the entity's rotation with camera's rotation
        if( rotateObject )
            transformed.Rotation = transformed.Rotation - Rotation;

        //// Adjust to camera position
        //trans.X += X * Zoom;
        //trans.Y += Y * Zoom;

        // Adjust to center of entity
        //trans.X -= drawable.Width / 2;
        //trans.Y -= drawable.Height / 2;
        //double centerX = X;
        //double centerY = Y;

    }

}