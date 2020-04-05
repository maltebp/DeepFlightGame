
using DeepFlight;
using DeepFlight.utility.KeyboardController;
using Microsoft.Xna.Framework;
using System;

public abstract class Scene : View {

    // To switch scene, a scene may "request" a switch to another scene
    public Scene RequestedScene { get; private set;  }

    public Scene(Camera camera) : base(camera) { }
    public Scene() : base(null) { }

    protected void RequestSceneSwitch(Scene scene) {
        if( RequestedScene != null )
            throw new Exception("A new scene is already requested");
        RequestedScene = scene;
    }

}
