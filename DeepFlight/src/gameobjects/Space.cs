

class Space : Drawable {

    public Space(double x, double y) : base(Textures.SQUARE, 1, 1) {
        X = x;
        Y = y;
        AddCollider(new RectCollider(this));
    }
  
}