


using DeepFlight;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

public class TextureView : View {

    public Texture2D Texture { get; private set; } = null;
    public Color Col { get; set; } = Color.White;

    public TextureView(Camera camera, Texture2D texture) : base(camera) {
        Width = texture.Width;
        Height = texture.Height;
        Texture = texture;
    }

    //public TextureView(Camera camera, Texture2D texture, float width, float height) : base(camera) {
    //    Width = width;
    //    Height = height;
    //    Texture = texture;
    //}

    public TextureView(Camera camera, Texture2D texture, Color col, double x, double y, float width, float height) : base(camera) {
        Texture = texture;
        X = x;
        Y = y;
        Width = width;
        Height = height;
        Col = col; 
    }

    protected override void OnDraw(Renderer renderer) {
        renderer.Draw(Camera, this);
    }
}


//// TODO: Move this to an appropriate place
//public class CollisionPoint : TextureView {

//    protected CollisionPoint(Point point) : base(Textures.CIRCLE, 1, 1) {
//        X = point.X;
//        Y = point.Y;
//        Col = Color.Blue;

//    }

//    public static LinkedList<CollisionPoint> GetCollisionPoints(Collidable collidable) {
//        var points = new LinkedList<CollisionPoint>();

//        foreach (Collider collider in collidable.GetColliders()) {
//            foreach (Point point in collider.GetCollisionPoints()) {
//                points.AddLast(new CollisionPoint(point));
//            }
//        }   

//        return points;
//    }
//}