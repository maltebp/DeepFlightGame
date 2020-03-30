using System;
using System.Collections.Generic;


// The absolute base class for game objects.
// Defines an object position, dimensions and scale.
public abstract class Entity {

    public double X { get; set; } = 0;
    public double Y { get; set; } = 0;

    public float Width { get; set; }
    public float Height { get; set; }

    private float rotation = 0f;
    public float Rotation {
        get => rotation;
        set => rotation = MathExtension.Mod(value, (float)(2 * Math.PI));
    }

    public float scale = 1.0f;


    protected Entity(float width, float height) {
        Width = width;
        Height = height;
    }

}