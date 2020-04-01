

using Microsoft.Xna.Framework;

class Ship : DrawableTexture {

    public Ship() : base(Textures.SHIP, 2, 4) {
        Resistance = 0.98f;
        Col = Color.Red;
        MaxVelocity = 5f;
        AddCollider(new TriangleCollider(this));
    }


}