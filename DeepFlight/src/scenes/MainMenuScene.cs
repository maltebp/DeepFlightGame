
using DeepFlight.rendering;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;

class MainMenuScene : Scene {

    private Camera ui = new Camera();
    //private DrawableTexture background = new DrawableTexture(Textures.SQUARE, 500, 500);
    private DrawableTexture title = new DrawableTexture(Textures.TITLE);

    private int[] resolutions = { 1280, 720, 1600, 900, 1920, 1080 };
    private int resolution = 2;

    public override void Initialize(Renderer renderer) { 
        DrawableTexture ship = new Ship();
        Console.WriteLine(ship);

        Console.WriteLine("Texture: {0}x{1}", Textures.TITLE.Width, Textures.TITLE.Height);
        title.Height = (int) (ScreenManager.BaseHeight / 6);
        title.Width = title.Height * (Textures.TITLE.Width/Textures.TITLE.Height);
        title.X = 0;
        title.Y = -ScreenManager.BaseHeight/5;

        Console.WriteLine(title);
    }

    public override void Draw(Renderer renderer) {

        renderer.Draw(ui, title);

        if (Keys.R.IsPressed()) {
            //resolution++;
            //if (resolution >= resolutions.Length/2) resolution = 0;
            //renderer.SetResolution(resolutions[resolution * 2], resolutions[resolution * 2 + 1]);
        }

       // renderer.Draw(ui, new DrawableTexture(Textures.CIRCLE, 10, 10));

    }

    public override void Update(double timeDelta) {
        
    }
}