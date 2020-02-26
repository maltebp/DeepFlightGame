

using System;

public abstract class Entity {

    public double X { get; set; } = 0;
    public double Y { get; set; } = 0;

    public float Width { get; private set; }
    public float Height { get; private set; }

    // TODO: Adjust rotation interval to 2 PI
    private float rotation = 0f;
    public float Rotation {
        get => rotation;
        set => rotation = MathExtension.Mod(value, (float)(2 * Math.PI)); }

    public float scale = 1.0f;

    protected Entity(float width, float height) {
        Width = width;
        Height = height;
    }

}