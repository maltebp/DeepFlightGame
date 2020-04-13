﻿

using Microsoft.Xna.Framework;

class Ship : TextureView {

    // TODO: Fix the base constructor call
    public Ship(Camera camera) : base(camera, Textures.SHIP, Color.White, 0,0, 4, 2) {
        Resistance = 0.97f;
        Col = Color.Red;
        MaxVelocity = 20f;
        AddCollider(new TriangleCollider(this));
    }

    protected override void OnUpdate(double deltaTime) {
        Mover.Move(this);
    }

}