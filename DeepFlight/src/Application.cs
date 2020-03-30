
using Microsoft.Xna.Framework;


// Central controller class of the application
// Switches between different scenes
class ApplicationController : Game {

    private Renderer renderer;
    private GraphicsDeviceManager graphics;
    private Scene currentScene = null;

    public ApplicationController() {
        graphics = new GraphicsDeviceManager(this);
    }

    // Is being run first
    protected override void Initialize() {
        SwitchScene(new MainMenuScene());
        base.Initialize();
    }


    // Is run after initialization
    protected override void LoadContent() {
        renderer = new Renderer(graphics);
        Textures.LoadTextures(graphics.GraphicsDevice, Content);
        base.LoadContent();
    }


    protected override void Update(GameTime gameTime) {

        // Check for scene switch
        if (currentScene.RequestedScene != null)
            SwitchScene(currentScene.RequestedScene);

        currentScene.Update(gameTime.ElapsedGameTime.TotalSeconds);
        base.Update(gameTime);
    }


    protected override void Draw(GameTime gameTime) {
        currentScene.Draw(renderer);
        renderer.Flush();      
        base.Draw(gameTime);
    }



    private void SwitchScene(Scene scene) {
        if (currentScene != null)
            currentScene.Terminate();

        currentScene = scene;
        currentScene.Initialize();
    }

}