
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;

class MainMenuScene : Scene {

    private DrawableTexture dot = new DrawableTexture(Textures.CIRCLE, 5, 5);
    private DrawableText title = new DrawableText("Deep Flight", Fonts.ARIAL, 24, Color.White, 0, 0);
    private Camera camera = new Camera();

    private int[] resolutions = { 1280, 720, 1600, 900, 1920, 1080 };
    private int resolution = 2;

    public override void Initialize(Renderer renderer) {
    }

    public override void Draw(Renderer renderer) {
        if (Keys.S.IsHeld()) {
            camera.Rotation -= 0.05f;
        }

        if (Keys.D.IsHeld()) {
            camera.Rotation += 0.05f;
        }

        if(Keys.C.IsPressed()) {
            camera.X = 0;
            camera.Y = 0;
            camera.Rotation = 0;
        }

        if (Keys.X.IsHeld() ) {
            camera.Zoom += 0.05f;
        }

        if (Keys.Z.IsHeld()) {
            camera.Zoom -= 0.05f;
        }

        float moveFactor = 2f;
        if( Keys.Up.IsHeld()) camera.Y -= moveFactor;
        if (Keys.Down.IsHeld()) camera.Y += moveFactor;
        if (Keys.Left.IsHeld()) camera.X -= moveFactor;
        if (Keys.Right.IsHeld()) camera.X += moveFactor;



        if (Keys.R.IsPressed()) {
            resolution++;
            if (resolution >= resolutions.Length/2) resolution = 0;
            renderer.SetResolution(resolutions[resolution * 2], resolutions[resolution * 2 + 1]);
        }


        
        //renderer.DrawText(camera, title);
        //title.Rotation = (float) Math.PI;
        title.X = Mouse.GetState().X;
        title.Y = Mouse.GetState().Y;
        //dot.X = Mouse.GetState().X;
        //dot.Y = Mouse.GetState().Y;
        dot.X = 0;
        dot.Y = 0;

        renderer.Draw(camera, new DrawableTexture(Textures.SQUARE, Color.Black, 10,10, 0, 0));
        renderer.Draw(camera, new DrawableTexture(Textures.SQUARE, Color.Green, 10,10, 250, -250));
        renderer.Draw(camera, new DrawableTexture(Textures.SQUARE, Color.Blue, 10,10, 250, 250));
        renderer.Draw(camera, new DrawableTexture(Textures.SQUARE, Color.Red, 10,10, -250, 250));
        renderer.Draw(camera, new DrawableTexture(Textures.SQUARE, Color.Orange, 10,10, -250, -250));

        renderer.Draw(camera, new DrawableText("Center",    Fonts.ARIAL, 24, Color.Black, 0, 0));
        renderer.Draw(camera, new DrawableText("Top-Right", Fonts.ARIAL, 24, Color.Black, 250, -250));
        renderer.Draw(camera, new DrawableText("Bot-Right",    Fonts.ARIAL, 24, Color.Black, 250, 250));
        renderer.Draw(camera, new DrawableText("Bot-Left",    Fonts.ARIAL, 24, Color.Black, -250, 250));
        renderer.Draw(camera, new DrawableText("Top-Left",    Fonts.ARIAL, 24, Color.Black, -250, -250));
    }

    public override void Update(double timeDelta) {
        
    }
}