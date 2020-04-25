

using Microsoft.Xna.Framework;

class Ship : TextureView {

    public Ship(Camera camera) : base(camera, Textures.SHIP, Color.White, 0,0, 5, 3.2f) {
        Resistance = 0.97f;
        Color = Color.White;
        MaxVelocity = 20f;
        AddCollider(new TriangleCollider(this));
    }

    protected override void OnUpdate(double deltaTime) {
        Mover.Move(this, deltaTime);
    }

}