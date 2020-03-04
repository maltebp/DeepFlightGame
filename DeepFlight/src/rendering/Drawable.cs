
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

public abstract class Drawable : Collidable {

    public Texture2D Texture { get; private set; }
    public Color Col { get; set; } = Color.White;

    protected Drawable(Texture2D texture, int width, int height) : base(width, height) {
        Texture = texture;
    }


}



public class CollisionPoint : Drawable {

    protected CollisionPoint(Point point) : base(Textures.CIRCLE, 1, 1) {
        X = point.X;
        Y = point.Y;
        Col = Color.Blue;

    }

    public static LinkedList<CollisionPoint> GetCollisionPoints(Collidable collidable) {
        var points = new LinkedList<CollisionPoint>();

        foreach( Collider collider in collidable.GetColliders() ) {
            foreach( Point point in collider.GetCollisionPoints() ) {
                points.AddLast(new CollisionPoint(point));
            }
        }

        return points;
    }
}