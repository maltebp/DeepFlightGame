

using Microsoft.Xna.Framework;

class Space : TextureView {

    public Space(double x, double y) : base(null, Textures.SQUARE, Color.White, 0,0, 1, 1) {
        X = x;
        Y = y;
        AddCollider(new RectCollider(this));
    }


    
}