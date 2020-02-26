

class Space : Drawable {
    private double v1;
    private float v2;

    public Space(double x, double y, float rotation) : base(Textures.SQUARE, 50, 50) {
        X = x;
        Y = y;
        Rotation = rotation;
    }
}