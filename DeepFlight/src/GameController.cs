
using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

class GameController : Game {

    private GraphicsDeviceManager graphics;
    private Renderer renderer;
    private Camera camera;
    private Ship ship;
    private Mover mover;

    public GameController() {
        graphics = new GraphicsDeviceManager(this);
        //graphics.ToggleFullScreen();

        camera = new Camera(graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight);
        mover = new Mover();
    }

    protected override void Initialize() {
        renderer = new Renderer(new SpriteBatch(GraphicsDevice));
        
       
        base.Initialize();
    }

    protected override void LoadContent() {
        Textures.LoadTextures(Content);


        // Ship must be created after loading textures
        ship = new Ship();

        

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


        if ( Keys.Space.IsHeld()) {
            double rotation = ship.Rotation + Math.PI * 1.5;
            ship.AccelerationY = (float) (Math.Sin(rotation) * 0.5);
            ship.AccelerationX = (float) (Math.Cos(rotation) * 0.5);
        }
        else {
            ship.AccelerationY = 0;
            ship.AccelerationX = 0;
        }

        mover.Move(ship);

        base.Update(gameTime);
    }



    protected override void Draw(GameTime gameTime) {
        GraphicsDevice.Clear(Color.Black);

        renderer.Draw(camera, ship);

        renderer.Flush();
      
        base.Draw(gameTime);
    }










}