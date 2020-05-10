using DeepFlight;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


/// <summary>
/// Displays a texture (image)
/// </summary>
public class TextureView : View {

    public Texture2D Texture { get; set; } = null;
    public Color Color { get; set; } = Color.White;

    public TextureView(Camera camera, Texture2D texture) : base(camera) {
        Width = texture.Width;
        Height = texture.Height;
        Texture = texture;
    }

    public TextureView(Camera camera, Texture2D texture, Color col, double x, double y, float width, float height) : base(camera) {
        Texture = texture;
        X = x;
        Y = y;
        Width = width;
        Height = height;
        Color = col; 
    }

    protected override void OnDraw(Renderer renderer) {
        renderer.Draw(Camera, this);
    }
}