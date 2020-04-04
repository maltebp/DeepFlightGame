
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

public class DrawableText : Movable {

    private SpriteFont font;
    public SpriteFont Font {
        get { return font; }
        set { font = value; UpdateSize(); }
    }

    private string text;
    public string Text {
        get { return text; }
        set { text = value; UpdateSize(); }
    }
        
    public Color Col { get; set; } = Color.White;

    public DrawableText(string text, SpriteFont font, Color col, double x, double y) : base(0,0) {
        this.text = text;
        this.font = font;
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
        Width = (int) Font.MeasureString(Text).X;
        Height = (int) Font.MeasureString(Text).Y;
    }
}