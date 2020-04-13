﻿
using DeepFlight;
using DeepFlight.rendering;
using DeepFlight.utility.KeyboardController;
using Microsoft.Xna.Framework;
using System;

public abstract class Scene : View {

    // To switch scene, a scene may "request" a switch to another scene
    public Scene RequestedScene { get; private set;  }
    public bool RequestedExit { get; private set; } = false;

    public Scene() : base(camera: new Camera(layer: 1f)){
        Width = (float) ScreenController.BaseWidth;
        Height = (float)ScreenController.BaseHeight;
    }

    protected void RequestSceneSwitch(Scene scene) {
        if( RequestedScene != null )
            throw new Exception("A new scene is already requested");
        RequestedScene = scene;
    }

    protected void RequestExit() {
        RequestedExit = true;
    }

}
