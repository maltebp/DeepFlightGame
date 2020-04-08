﻿

using Microsoft.Xna.Framework;

class Ship : TextureView {

    // TODO: Fix the base constructor call
    public Ship() : base(null, Textures.SHIP, Color.White, 0,0, 2, 4) {
        Resistance = 0.98f;
        Col = Color.Red;
        MaxVelocity = 5f;
        AddCollider(new TriangleCollider(this));
    }


}