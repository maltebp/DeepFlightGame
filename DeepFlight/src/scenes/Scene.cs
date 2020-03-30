
using Microsoft.Xna.Framework;
using System;

public abstract class Scene {

    // To switch scene, a scene may "request" a switch to another scene
    public Scene RequestedScene { get; private set;  }

    protected void RequestSceneSwitch(Scene scene) {
        if( RequestedScene != null )
            throw new Exception("A new scene is already requested");
        RequestedScene = scene;
    }

    // First method which is run, once the scene has been "switched" in
    public virtual void Initialize() { }

    // Is run once the scene is "switched out"
    public virtual void Terminate() { }

    public abstract void Update(double timeDelta);

    public abstract void Draw(Renderer renderer);
}
