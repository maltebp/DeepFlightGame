


using System;

class Camera {

    public double X { get; set; } = 0;
    public double Y { get; set; } = 0;

    public int Width { get; private set; }
    public int Height { get; private set; }

    private float rotation = 0f; // Radians
    public float Rotation {
        get => rotation;
        set => rotation = MathExtension.Mod(value, (float)(2 * Math.PI));
    }

    public float Zoom { get; set; } = 1f;

    public Camera(int width, int height) {
        Width = width;
        Height = height;
    }

}