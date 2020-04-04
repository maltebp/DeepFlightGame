
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

public class DrawableText : Movable {

    private Font font;
    public Font Font {
        get { return font; }
        set { font = value; UpdateSize(); }
    }

    private string text;
    public string Text {
        get { return text; }
        set { text = value; UpdateSize(); }
    }
        
    public Color Col { get; set; } = Color.White;

    // Font size
    private float size;
    public float Size {
        get => size;
        set { size = value < 0 ? 0 : value; UpdateSize(); }
    }


    public DrawableText(string text, Font font, float size, Color col, double x, double y) : base(0,0) {
        this.text = text;
        this.font = font;
        this.size = size;
        X = x;
        Y = y;
        Col = col;
        UpdateSize();
    }

    public DrawableText(DrawableText original) : base(original) {
        text = original.Text;
        font = original.Font;
        Col = original.Col;
    }

    private void UpdateSize() {
        Width = (int) Font.MeasureString(Text, size).X;
        Height = (int) Font.MeasureString(Text, size).Y;
    }
}