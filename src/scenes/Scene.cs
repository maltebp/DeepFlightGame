using DeepFlight;
using DeepFlight.rendering;
using System;


/// <summary>
/// The Application will display one Scene at a time, and a Scene
/// may be added children, which will follow the Scene logic (will
/// be removed, if the scene is removed i.e.)
/// </summary>
public abstract class Scene : View {

    // To switch scene, a scene may "request" a switch to another scene
    public Scene RequestedScene { get; private set;  }
    public bool RequestedExit { get; private set; } = false;

    public Scene() : base(camera: new Camera(layer: 1f)){
        Width = (float) ScreenController.BaseWidth+4; 
        Height = (float)ScreenController.BaseHeight+4;
        // Note on extra width and height:
        // Had some trouble with background not filling entire space
    }

    // Requests that the scene should be switched out with
    // another scene
    protected void RequestSceneSwitch(Scene scene) {
        if( RequestedScene != null )
            throw new Exception("A new scene is already requested");
        RequestedScene = scene;
    }

    // Sets the Scene exit flag, telling the APplication
    // that this scene would like to exit the game
    protected void RequestExit() {
        RequestedExit = true;
    }

}
