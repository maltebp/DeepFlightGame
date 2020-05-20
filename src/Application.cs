
using DeepFlight.rendering;
using DeepFlight.utility.KeyboardController;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using DeepFlight.scenes;
using DeepFlight.src.gui.debugoverlay;
using System.Diagnostics;
using Microsoft.Xna.Framework.Graphics;
using DeepFlight.utility;


// Central controller class of the application
// Switches between different scenes
public class Application : Game {

    private Renderer renderer;
    private GraphicsDeviceManager graphics;
    private Scene currentScene = null;

    private FPSCounter fpsCounter = new FPSCounter();

    // To keep track of which screen resolution we are at
    private int resolutionIndex = 0;

    public delegate void DrawEventHandler(Renderer renderer);
    public static event DrawEventHandler DrawEvent;

    public delegate void UpdateEventHandler(double deltaTime);
    public static event UpdateEventHandler UpdateEvent;

    public Application() {
        graphics = new GraphicsDeviceManager(this);
        IsFixedTimeStep = false;
    }   

    // Is being run first
    protected override void Initialize() {
        base.Initialize();
    }


    // Is run after initialization
    protected override void LoadContent() {

        // Setup keyboard controller
        // The Application registers itself to the KeyboardController's
        // character and key input events 
        KeyboardController.Initialize(Window);
        KeyboardController.KeyEvent += OnKeyInput;
        KeyboardController.CharEvent += OnCharInput;

    
        ScreenController.Initialize(graphics);

        // Initialize Renderer and loads fonts and textures
        renderer = new Renderer(graphics);
        Textures.LoadTextures(graphics.GraphicsDevice, Content);
        Font.LoadAll(Content);
                
        // We can't add it as a Child view since 'Application' is not
        // a View, so we just initialize it manually
        DebugOverlay.Instance.Initialize();
        DebugOverlay.Instance.Hidden = true;

        // Some global debug info
        DebugOverlay.Info.AddInfoLine("Scene", "No information yet", (infoLine) => infoLine.Info = currentScene.GetType().Name);
        DebugOverlay.Info.AddInfoLine("FPS", "No information yet", (infoLine) => infoLine.Info = string.Format("{0:N1}", fpsCounter.GetFPS()));

        SwitchScene(new LoginScene());

        base.LoadContent();
    }


    protected override void Update(GameTime gameTime) {
        KeyboardController.UpdateState();

        if (currentScene.RequestedExit)
            Exit();

        // Check for scene switch
        if (currentScene.RequestedScene != null)
            SwitchScene(currentScene.RequestedScene);

        UpdateEvent(gameTime.ElapsedGameTime.TotalSeconds);
        
        // Since DebugOverlay is not a Child view, we have to update it manually
        DebugOverlay.Instance.Update(gameTime.ElapsedGameTime.TotalSeconds);

        base.Update(gameTime);
    }


    protected override void Draw(GameTime gameTime) {
        fpsCounter.Update(gameTime);
        DrawEvent(renderer);
        renderer.Flush();      
        base.Draw(gameTime);
    }


    private void OnKeyInput(KeyEventArgs e) {
        // Prevent key input if the game is minimized
        if( IsActive ) {
            if (e.Action == KeyAction.PRESSED) {
                if (e.Key == Keys.F1) {
                    DebugOverlay.Instance.Hidden = !DebugOverlay.Instance.Hidden;
                    return; // Consume event
                }
            }

            // Forward key event to scene
            currentScene.KeyInput(e);
        }
    }

    private void OnCharInput(CharEventArgs e) {
        currentScene.CharInput(e);
    }

    private void SwitchScene(Scene scene) {
        if (currentScene != null)
            currentScene.Terminate();

        currentScene = scene;
        currentScene.Focused = true;
        currentScene.Hidden = false;

        Console.WriteLine("\nSwitching scene: '{0}'", scene.GetType().Name);
        currentScene.Initialize();
    }


 
    protected override void OnExiting(object sender, EventArgs args) {
        /* NEEDS TO BE HERE
        /* Bug in MonoGame OpenGL version where it doesn't close the 
            window correctly on Exit. Workaround is to kill process
            before it gets to. */
        Process.GetCurrentProcess().Kill();
    }

}