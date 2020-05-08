using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;


// Collidable is an object, which may be attached "Colliders", and thus
// may collide with other Collidable objects.
public abstract class Collidable : Entity {

    private LinkedList<Collider> colliders = new LinkedList<Collider>();

    protected Collidable(float width, float height) : base(width, height) { }


    protected void AddCollider(Collider collider) {
        colliders.AddLast(collider);
    }

    public LinkedList<Collider> GetColliders() {
        return colliders;
    }

    /// <summary>
    /// Checks if any of this Collidable's colliders collide with any of the
    /// given Collidable's colliders.
    /// </summary>
    public bool CollidesWith(Collidable collidable) {
        foreach( Collider thisCollider in colliders) {
            foreach (Collider targetCollider in collidable.GetColliders())
                if (thisCollider.CollidesWith(targetCollider))
                    return true;
        }
        return false;
    }
    
}


/// <summary>
/// A Collider defines an area with which you can collide. In general it supplies a method
/// which returns a set of "collision points" for other colliders to use, to see if they
/// collide with this, and a method to see if the it collides with any points.
/// 
/// Thus the following holds: ColliderA.CollidesWith(ColliderB) != ColliderB.CollidesWith(ColliderA)
/// </summary>
public abstract class Collider {

    protected Entity entity;

    protected Collider(Entity entity) {
        this.entity = entity;
    }

    
    /// <summary>
    /// Tests whether this Collider collides with any of the the points
    /// from the parameter Collider
    /// </summary>
    /// <param name="collider"></param>
    /// <returns></returns>
    public bool CollidesWith(Collider collider) {
        // Check if collision is even possible (i.e. they are way too far from each other)
        var center = new Point(entity.GetCenterX(), entity.GetCenterY());
        if (!collider.IsCollisionPossible(center, FilterDistance()))
            return false;   
        // Do the actual collision check
        return CollidesWithPoints(collider.GetCollisionPoints());
    }


    protected bool IsCollisionPossible(Point filterCenter, double filterDistance) {
        return filterCenter.DistanceTo(new Point(entity.GetCenterX(), entity.GetCenterY() )) - filterDistance - FilterDistance() <= 0;
    }

    /// <summary>
    /// Retrieves an of Collision points. Any of the points represents point
    /// where this Collidable may collide with something.
    /// </summary>
    public abstract Point[] GetCollisionPoints();

    /// <summary>
    /// Checks if any of the points given collides with this Collidable.
    /// </summary>
    protected  abstract bool CollidesWithPoints(params Point[] points);


    protected abstract double FilterDistance();

}


// Collider which detects collision within a radius.
// Radius is determined by either Entity width or height, depending on which is largest.
public class CircleCollider : Collider {

    public float Scale { get; set; }

    public CircleCollider(Entity entity) : base(entity) { }
    public CircleCollider(Entity entity, float scale) : base(entity) {
        Scale = scale;
    }

    public override Point[] GetCollisionPoints() {
        var radius = (entity.Width > entity.Height ? entity.Width : entity.Height) / 2 * Scale;

        int numberOfPoints = (int)radius * 12;
       
        Point[] points = new Point[ numberOfPoints ];

        var centerX = entity.GetCenterX();
        var centerY = entity.GetCenterY();
        var angle = 0.0;
        var angleStep = (2 * Math.PI) / numberOfPoints;
        for(int i=0; i<numberOfPoints; i++) {
            points[i] = new Point(centerX + Math.Cos(angle) * radius, centerY + Math.Cos(angle) * radius);
            angle += angleStep;
        }

        return points;
    }

    protected override bool CollidesWithPoints( params Point[] points) {
        var radius = (entity.Width > entity.Height ? entity.Width : entity.Height) / 2 * Scale;
        var centerX = entity.GetCenterX();
        var centerY = entity.GetCenterY();
        foreach( Point point in points) {
            var distance = Math.Sqrt(Math.Pow(centerX - point.X, 2) + Math.Pow(centerY - point.Y, 2));
            if (distance < radius)
                return true;
        }
        return false;
    }


    protected override double FilterDistance() {
        return (entity.Width > entity.Height ? entity.Width : entity.Height);
    }
}


// Collider which detects collision within rectangular boundary
public class RectCollider : Collider {

    public RectCollider(Entity entity) : base(entity) { }


    // Points are:
    //
    //  p0------p2 
    //   |      |
    //   |      |
    //  p1------p3
    //
    public override Point[] GetCollisionPoints() {
        var centerX = entity.GetCenterX();
        var centerY = entity.GetCenterY();
        double x1 = centerX - entity.Width / 2;
        double x2 = centerX + entity.Width / 2;
        double y1 = centerY - entity.Height / 2;
        double y2 = centerY + entity.Height / 2;

        return new Point[]{
            new Point(x1, y1),
            new Point(x1, y2),
            new Point(x2, y1),
            new Point(x2, y2)
        };
    }

    protected override bool CollidesWithPoints(params Point[] points) {
        var centerX = entity.GetCenterX();
        var centerY = entity.GetCenterY();
        double x1 = centerX - entity.Width / 2;
        double x2 = centerX + entity.Width / 2;
        double y1 = centerY - entity.Height / 2;
        double y2 = centerY + entity.Height / 2;

        foreach (Point point in points) {
            if (point.X >= x1 && point.X <= x2 && point.Y >= y1 && point.Y <= y2)
                return true;

        }
        return false;
    }

    protected override double FilterDistance() {
        return (entity.Width > entity.Height ? entity.Width : entity.Height);
    }
}


/// <summary>
/// Collider describing a triangular area.
/// </summary>
public class TriangleCollider : Collider {

    public TriangleCollider(Entity entity) : base(entity) { }

    public override Point[] GetCollisionPoints() {
        Point center = new Point(entity.X, entity.Y);
       
        // Calculate the points of the Triangle, where
        // the points are: 
        //     p0    
        //    /  \
        //   /    \
        // p2------p1
        return new Point[]{
            // p0
            new Point(center.X, center.Y - entity.Height / 2).Rotate(center, entity.Rotation),
            // p1
            new Point(center.X+entity.Width/2, center.Y + entity.Height/2).Rotate(center, entity.Rotation),
            // p2
            new Point(center.X-entity.Width/2, center.Y + entity.Height/2).Rotate(center, entity.Rotation)
        };
    }

    protected override bool CollidesWithPoints(params Point[] points) {
        // TODO: Implement triangle collision detection
        // Source: https://stackoverflow.com/questions/2049582/how-to-determine-if-a-point-is-in-a-2d-triangle
        // Note: It's rather non-trivial to implement, and it's strictly not necessary (as it isn't used), so
        // this is left as not-implemented
        throw new Exception("Don't call ColllidesWith on a Triangle collider - not implemented yet!");
    }

    protected override double FilterDistance() {
        return (entity.Width > entity.Height ? entity.Width : entity.Height);
    }
}