using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;


// Collidable is an object, which may be attached "Colliders", and thus
// may collide with other Collidable objects.
public abstract class Collidable : Entity {

    private LinkedList<Collider> colliders = new LinkedList<Collider>();

    protected Collidable(int width, int height) : base(width, height) { }

    /// <summary>
    /// Deep copy constructor.
    /// WARNING: Colliders are not copied.
    /// </summary>
    public Collidable(Collidable original) : base(original) {
        // TODO: WARNING!!! Colliders are not copied
    }

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

// TODO: Remove this
//public interface Collidable {
//    bool CollidesWith(Collidable collidable);
//    List<Collider> GetColliders();
//}

public abstract class Collider {

    protected Entity entity;

    protected Collider(Entity entity) {
        this.entity = entity;
    }

    
    public bool CollidesWith(Collider collider) {
        var center = new Point(entity.X, entity.Y);
        if (!collider.IsCollisionPossible(center, FilterDistance()))
            return false;
        return CollidesWithPoints(collider.GetCollisionPoints());
    }


    protected bool IsCollisionPossible(Point filterCenter, double filterDistance) {
        return filterCenter.DistanceTo(new Point(entity.X, entity.Y)) - filterDistance - FilterDistance() <= 0;
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
public class CircleCollidable : Collider {

    public CircleCollidable(Entity entity) : base(entity) { }

    public override Point[] GetCollisionPoints() {
        var radius = (entity.Width > entity.Height ? entity.Width : entity.Height) / 2;

        int numberOfPoints = (int)radius * 12;
       
        Point[] points = new Point[ numberOfPoints ];

        var angle = 0.0;
        var angleStep = (2 * Math.PI) / numberOfPoints;
        for(int i=0; i<numberOfPoints; i++) {
            points[i] = new Point(entity.X + Math.Cos(angle) * radius, entity.Y + Math.Cos(angle) * radius); 
        }

        return points;
    }

    protected override bool CollidesWithPoints( params Point[] points) {
        var radius = (entity.Width > entity.Height ? entity.Width : entity.Height) / 2;
        foreach( Point point in points) {
            var distance = Math.Sqrt(Math.Pow(entity.X - point.X, 2) + Math.Pow(entity.Y - point.Y, 2));
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

    public override Point[] GetCollisionPoints() {
        double x1 = entity.X - entity.Width / 2;
        double x2 = entity.X + entity.Width / 2;
        double y1 = entity.Y - entity.Height / 2;
        double y2 = entity.Y + entity.Height / 2;

        return new Point[]{
            new Point(x1, y1),
            new Point(x1, y2),
            new Point(x2, y1),
            new Point(x2, y2)
        };
    }

    protected override bool CollidesWithPoints(params Point[] points) {
        double x1 = entity.X - entity.Width / 2;
        double x2 = entity.X + entity.Width / 2;
        double y1 = entity.Y - entity.Height / 2;
        double y2 = entity.Y + entity.Height / 2;

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




public class TriangleCollider : Collider {

    public TriangleCollider(Entity entity) : base(entity) { }

    public override Point[] GetCollisionPoints() {
        Point center = new Point(entity.X, entity.Y);

        return new Point[]{
            new Point(entity.X, entity.Y - entity.Height / 2).Rotate(center, entity.Rotation),
            new Point(entity.X+entity.Width/2, entity.Y + entity.Height/2).Rotate(center, entity.Rotation),
            new Point(entity.X-entity.Width/2, entity.Y + entity.Height/2).Rotate(center, entity.Rotation)
        };
    }

    protected override bool CollidesWithPoints(params Point[] points) {

        // TODO: Implement triangle collision detection
        // Source: https://stackoverflow.com/questions/2049582/how-to-determine-if-a-point-is-in-a-2d-triangle
        throw new Exception("Don't call ColllidesWith on a Triangle collider - not implemented yet!");
    }

    protected override double FilterDistance() {
        return (entity.Width > entity.Height ? entity.Width : entity.Height);
    }
}