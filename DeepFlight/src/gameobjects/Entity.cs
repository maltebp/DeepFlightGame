using System;
using System.Collections.Generic;


// The absolute base class for game objects.
// Defines an object position, dimensions and scale.
public class Entity {

    public double X { get; set; } = 0;
    public double Y { get; set; } = 0;

    public float Width { get; set; } = 1f;
    public float Height { get; set; } = 1f;

    private float rotation = 0f;
    public float Rotation {
        get => rotation;
        set => rotation = MathExtension.Mod(value, (float)(2 * Math.PI));
    }

    public float scale = 1.0f;


    public Entity() { }

    /// <summary>
    /// Create an entity as a copy of another
    /// </summary>
    public Entity(Entity entity) {
        X = entity.X;
        Y = entity.Y;
        Width = entity.Width;
        Height = entity.Height;
        Rotation = entity.Rotation;
    }

    public Entity(float width, float height) {
        Width = width;
        Height = height;
    }

    public override string ToString() {
        return string.Format(
            "Entity( " +
            "x={0}, " +
            "y={1}, " +
            "width={2}, " +
            "height={3}, " +
            "rotation={4}" +
            " )",
            X, Y, Width, Height, Rotation
           );
    }
}