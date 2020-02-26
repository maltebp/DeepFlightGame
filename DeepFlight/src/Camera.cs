


class Camera {

    public double X { get; set; } = 0;
    public double Y { get; set; } = 0;

    public int Width { get; private set; }
    public int Height { get; private set; }

    // TODO: Adjust rotation interval to 2 PI
    public float Rotation { get; set; } = 0f;
    public float Zoom { get; set; } = 1f;

    public Camera(int width, int height) {
        Width = width;
        Height = height;
    }

}