

using Microsoft.Xna.Framework;

class Ship : TextureView {

    public Ship(Camera camera) : base(camera, Textures.SHIP, new Color(50,50,50), 0,0, 5, 3.2f) {
        Resistance = 0.97f;
        MaxVelocity = 20f;
        AddCollider(new TriangleCollider(this));
    }

    protected override void OnUpdate(double deltaTime) {
        Mover.Move(this, deltaTime);
    }

}