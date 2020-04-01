
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;

class MainMenuScene : Scene {

    private DrawableTexture dot = new DrawableTexture(Textures.CIRCLE, 5, 5);
    private DrawableText title = new DrawableText("Deep Flight", Fonts.ARIAL.GetFont(24), Color.White, 0, 0);
    private Camera camera = new Camera();

    public override void Initialize(Renderer renderer) {
    }

    public override void Draw(Renderer renderer) {
        if (Keys.Left.IsHeld()) {
            title.Rotation -= 0.05f;
        }

        if (Keys.Right.IsHeld()) {
            title.Rotation += 0.05f;
        }

        
        renderer.DrawText(camera, title);
        //title.Rotation = (float) Math.PI;
        title.X = Mouse.GetState().X;
        title.Y = Mouse.GetState().Y;
        dot.X = Mouse.GetState().X;
        dot.Y = Mouse.GetState().Y;
        renderer.DrawTexture(camera, dot);

        
    }

    public override void Update(double timeDelta) {
        
    }
}