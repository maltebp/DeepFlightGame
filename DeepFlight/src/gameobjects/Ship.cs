

using Microsoft.Xna.Framework;

class Ship : Movable {


    public Ship() : base(Textures.SHIP, 2, 4, 5f) {
        Resistance = 0.98f;
        Col = Color.Red;
    }

}