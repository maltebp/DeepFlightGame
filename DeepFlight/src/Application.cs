
using DeepFlight.rendering;
using DeepFlight.utility.KeyboardController;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;


// Central controller class of the application
// Switches between different scenes
public class ApplicationController : Game {

    private Renderer renderer;
    private GraphicsDeviceManager graphics;
    private Scene currentScene = null;

    // To keep track of which screen resolution we are at
    private int resolutionIndex = 0;

    public delegate void DrawEventHandler(Renderer renderer);
    public event DrawEventHandler DrawEvent;

    public delegate void UpdateEventHandler(double deltaTime);
    public event UpdateEventHandler UpdateEvent;

    public ApplicationController() {
        graphics = new GraphicsDeviceManager(this);
    }   

    // Is being run first
    protected override void Initialize() {
        base.Initialize();
    }


    // Is run after initialization
    protected override void LoadContent() {

        // Setup keyboard controller
        KeyboardController.Initialize(Window);
        KeyboardController.KeyEvent += OnKeyInput;
        KeyboardController.CharEvent += OnCharInput;

        ScreenManager.Initialize(graphics);


        renderer = new Renderer(graphics);
        Textures.LoadTextures(graphics.GraphicsDevice, Content);
        Fonts.Load(Content);

        SwitchScene(new MainMenuScene());
        base.LoadContent();
    }


    protected override void Update(GameTime gameTime) {
        KeyboardController.UpdateState();

        // Check for scene switch
        if (currentScene.RequestedScene != null)
            SwitchScene(currentScene.RequestedScene);

        UpdateEvent(gameTime.ElapsedGameTime.TotalSeconds);
        base.Update(gameTime);
    }

    //public void OnKeyEvent(KeyEventArgs eventArgs) {
    //    if (eventArgs.Handled) return;

    //    if (eventArgs.Action == KeyAction.PRESSED) {
    //        var key = eventArgs.Key;
    //        if (key == Keys.Escape) {
    //            Exit();
    //            eventArgs.Handled = true;
    //        }
    //        if (key == Keys.F1) {
    //            Resolution[] resList = ScreenManager.availableResolutions;
    //            resolutionIndex = ++resolutionIndex % resList.Length;
    //            ScreenManager.SetResolution(resList[resolutionIndex]);
    //            eventArgs.Handled = true;
    //        }
    //    }

       
    //}


    protected override void Draw(GameTime gameTime) {
        DrawEvent(renderer);
        renderer.Flush();      
        base.Draw(gameTime);
    }


    private void OnKeyInput(KeyEventArgs e) {
        System.Console.WriteLine("KeyEvent: " + e);
        currentScene.KeyInput(e);
    }

    private void OnCharInput(CharEventArgs e) {
        System.Console.WriteLine("CharEvent: " + e);
        currentScene.CharInput(e);
    }

    private void SwitchScene(Scene scene) {
        if (currentScene != null)
            currentScene.Terminate(this);

        currentScene = scene;
        currentScene.Initialize(this);
    }

}