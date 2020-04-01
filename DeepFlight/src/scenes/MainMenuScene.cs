
using Microsoft.Xna.Framework;


class MainMenuScene : Scene {

    private DrawableTexture dot = new DrawableTexture(Textures.CIRCLE, 10, 10);
    private DrawableText title = new DrawableText("Deep Flight", Fonts.ARIAL.GetFont(24), Color.White, 100, 100);
    private Camera camera = new Camera();

    public override void Initialize(Renderer renderer) {
    }

    public override void Draw(Renderer renderer) {
        renderer.DrawText(camera, title);
        //dot.X = 0;
        //dot.Y = 0;
        //renderer.DrawTexture(camera,dot);
    }

    public override void Update(double timeDelta) {
        
    }
}