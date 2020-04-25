using System;
using System.Collections.Generic;
using System.ComponentModel;


// The absolute base class for game objects.
// Defines an object position, dimensions and scale.
public class Entity  {

    public virtual double X { get; set; } = 0;
    public virtual double Y { get; set; } = 0;
    

    public virtual float Width { get; set; } = 1f;
    public virtual float Height { get; set; } = 1f;

    private float rotation = 0f;
    public virtual float Rotation {
        get => rotation;
        set => rotation = MathExtension.Mod(value, (float)(2 * Math.PI));
    }

    /// <summary>
    ///  WARNING: Probably shouldn't this too much (not fully integrated)
    /// </summary>
    public float scale = 1.0f;

    public HorizontalOrigin HOrigin { get; set; } = HorizontalOrigin.CENTER;
    public VerticalOrigin VOrigin { get; set; } = VerticalOrigin.CENTER;

    public Entity() { }

    /// <summary>
    /// Create an entity as a copy of another
    /// </summary>
    public Entity(Entity entity) {
        Inherit(entity);
    }

    public Entity(float width, float height) {
        Width = width;
        Height = height;
    }

    public double GetCenterX() {
        if (HOrigin == HorizontalOrigin.LEFT)
            return X + Width/2f;
        if (HOrigin == HorizontalOrigin.RIGHT )
            return X - Width/2f;
        return X;
    }

    public double GetCenterY() {
        if (VOrigin == VerticalOrigin.TOP)
            return Y + Height / 2f;
        if (VOrigin == VerticalOrigin.BOTTOM)
            return Y - Height / 2f;
        return Y;
    }

    public void Inherit(Entity entity) {
        X = entity.X;
        Y = entity.Y;
        Width = entity.Width;
        Height = entity.Height;
        Rotation = entity.Rotation;
        HOrigin = entity.HOrigin;
        VOrigin = entity.VOrigin;
        scale = entity.scale;
    }

    public override string ToString() {
        return string.Format(
            "Entity( " +
            "x={0}, " +
            "y={1}, " +
            "width={2}, " +
            "height={3}, " +
            "rotation={4}, " +
            "h.origin={5}, " +
            "v.origin={6} )",
            X, Y, Width, Height, Rotation, HOrigin.GetName(), VOrigin.GetName()
           );
    }

    
}


public enum HorizontalOrigin {
    LEFT, CENTER, RIGHT,
}

public enum VerticalOrigin {
    TOP, CENTER, BOTTOM
}

public static class OriginMethods {

    /// <summary>
    /// Gets the name of the given VerticalOrigin, as it's
    /// written in the code (i.e. CENTER = "CENTER").
    /// </summary>
    public static string GetName(this HorizontalOrigin origin) {
        return Enum.GetName(typeof(HorizontalOrigin), origin);
    }

    /// <summary>
    /// Gets the name of the given VerticalOrigin, as it's
    /// written in the code (i.e. CENTER = "CENTER").
    /// </summary>
    public static string GetName(this VerticalOrigin origin) {
        return Enum.GetName(typeof(VerticalOrigin), origin);
    }
}