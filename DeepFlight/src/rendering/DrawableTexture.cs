


using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

public class DrawableTexture : Movable {

    public Texture2D Texture { get; private set; } = null;
    public Color Col { get; set; } = Color.White;

    public DrawableTexture(Texture2D texture, int width, int height) : base(width, height) {
        Texture = texture;
    }
}


// TODO: Move this to an appropriate place
public class CollisionPoint : DrawableTexture {

    protected CollisionPoint(Point point) : base(Textures.CIRCLE, 1, 1) {
        X = point.X;
        Y = point.Y;
        Col = Color.Blue;

    }

    public static LinkedList<CollisionPoint> GetCollisionPoints(Collidable collidable) {
        var points = new LinkedList<CollisionPoint>();

        foreach (Collider collider in collidable.GetColliders()) {
            foreach (Point point in collider.GetCollisionPoints()) {
                points.AddLast(new CollisionPoint(point));
            }
        }

        return points;
    }
}