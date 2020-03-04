
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

class GameController : Game {

    private GraphicsDeviceManager graphics;
    private Renderer renderer;
    private Camera camera;
    private Ship ship;
    private Track track;
    private Mover mover;
    private LinkedList<Space> spaces = new LinkedList<Space>();

    public GameController() {
        graphics = new GraphicsDeviceManager(this);
        graphics.PreferredBackBufferWidth = 1080;
        graphics.PreferredBackBufferHeight = 720;
        //graphics.ToggleFullScreen();

        camera = new Camera(graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight);
        mover = new Mover();
    }

    protected override void Initialize() {
        renderer = new Renderer(new SpriteBatch(GraphicsDevice));
        track = Generator.GenerateTrack();        
       
        base.Initialize();
    }

    protected override void LoadContent() {
        Textures.LoadTextures(graphics.GraphicsDevice, Content);


        // Ship must be created after loading textures
        ship = new Ship();

        Random rand = new Random();
        for (int i = 0; i < 100; i++) {
            spaces.AddLast(new Space(rand.Next(-100, 100), rand.Next(-100, 100)));
        }


        base.LoadContent();
    }

    protected override void Update(GameTime gameTime) {

        InputController.UpdateState();

        
        

        // TODO: Replace this with a better solution
        if (Keys.Escape.IsPressed())
            Exit();



        if (Keys.Left.IsHeld()) {
            ship.Rotation -= 0.05f;
        }

        if (Keys.Right.IsHeld()) {
            ship.Rotation += 0.05f;
        }


        
        //camera.Rotation = ship.Rotation;

        //if (Keys.W.IsHeld()) {
        //    camera.Rotation -= 0.05f;
        //}

        //if (Keys.E.IsHeld()) {
        //    camera.Rotation += 0.05f;
        //}


        if( Keys.F.IsPressed()) {
            camera.Zoom = 10 + (camera.Zoom + 2.5f) % 10;
            
        }

        if ( Keys.Space.IsHeld()) {
            double rotation = ship.Rotation + Math.PI * 1.5;
            ship.AccelerationY = (float) (Math.Sin(rotation) * 0.05);
            ship.AccelerationX = (float) (Math.Cos(rotation) * 0.05);
        }
        else {
            ship.AccelerationY = 0;
            ship.AccelerationX = 0;
        }

        mover.Move(ship);
        camera.X = ship.X;
        camera.Y = ship.Y;

        base.Update(gameTime);
    }


    protected override void Draw(GameTime gameTime) {
        GraphicsDevice.Clear(Color.Black);


        track.ForBlocksInRange((int)(-100 + camera.X), (int)(-100 + camera.Y), (int)(100 + camera.X), (int)(100 + camera.Y), (type, x, y) => {
            Space space = new Space(0, 0);
            space.X = x;
            space.Y = y;
            renderer.Draw(camera, space);
        });

        renderer.Draw(camera, ship);

        //foreach (Space space in spaces) {
        //    renderer.Draw(camera, space);
        //}



        renderer.Flush();
      
        base.Draw(gameTime);
    }










}