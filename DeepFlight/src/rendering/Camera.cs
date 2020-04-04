


using System;

public class Camera {

    public double X { get; set; } = 0;
    public double Y { get; set; } = 0;

    private float rotation = 0f; // Radians
    public float Rotation {
        get => rotation;
        set => rotation = MathExtension.Mod(value, (float)(2 * Math.PI));
    }

    // Percentage scaling of what the camera sees
    // Can't go below 0
    public float zoom = 1f;
    public float Zoom {
        get => zoom;
        set => zoom = value <= 0 ? 0.001f : value;
    }

    /// <summary>
    /// Transform the given Entity to the camera view, by
    /// scaling, moving and rotation the entity
    /// </summary>
    /// <param name="transformed">The Entity object to be transformed</param>
    /// <param name="rotateObject">Whether or not to rotate the object with the camera
    /// (i.e. it might be useful to not rotate text)</param>
    public void Transform(Entity transformed, bool rotateObject) {

        //// Adjust to camera view
        transformed.X -= X;
        transformed.Y -= Y;

        //// Apply camera zoom
        transformed.X *= Zoom;
        transformed.Y *= Zoom;

        // Don't perform the scaling here, but adjust it to the zoom
        transformed.scale *= Zoom;

        // Rotate the object around camera origin (0,0)
        double cos = Math.Cos(-Rotation);
        double sin = Math.Sin(-Rotation);
        double x = cos * transformed.X - sin * transformed.Y;
        double y = sin * transformed.X + cos * transformed.Y;
        transformed.X = x;
        transformed.Y = y;

        // Adjust the entity's rotation with camera's rotation
        if( rotateObject )
            transformed.Rotation = transformed.Rotation - Rotation;
    }

}