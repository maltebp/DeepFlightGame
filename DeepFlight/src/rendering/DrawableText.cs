﻿
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
    private double size;
    public double Size {
        get => size;
        set { size = value < 0 ? 0 : value; UpdateSize(); }
    }


    public DrawableText(string text, Font font, double size, Color col, double x, double y) : base(0,0) {
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

        if( Text != null && Text.Length > 0){
            Width = Font.MeasureString(Text, size).X;
        }
        else {
            Width = 0;
        }

        // Height should always be the same (regardless of which characters)
        Height = Font.MeasureString("|LAJ*", size).Y;
    }

    public override string ToString() {
        return string.Format(
            "DrawableText( " +
            "text={0}, " +
            "font={1}, " +
            "size={2}, " +
            "x={3}, " +
            "y={4}, " +
            "width={5}, " +
            "height={6}, " +
            "rotation={7}, " +
            "h.origin={8}, " +
            "v.origin={9} )",
            Text, Font.Name, Size, X, Y, Width, Height, Rotation, HOrigin.GetName(), VOrigin.GetName()
           );
    }
}