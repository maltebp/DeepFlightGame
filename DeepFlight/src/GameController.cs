
using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;


class GameController : Game {

    private static readonly bool SAVE_TRACK = true;
    private static readonly bool LOAD_TRACK = true;
    private static readonly bool RANDOM_TRACK = true;
    private static readonly int TRACK_SEED = 2000; // Seed to use if RANDOM_TRACK is false

    // Zoom Settings    
    private static readonly float ZOOM_DEFAULT = 8f;
    private static readonly float ZOOM_MAX = 30f;
    private static readonly float ZOOM_MIN = 0.5f;
    private static readonly float ZOOM_FACTOR = 0.001f; // How "fast" to zoom

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
        camera.Zoom = ZOOM_DEFAULT;

        mover = new Mover();
    }

    protected override void Initialize() {
        renderer = new Renderer(new SpriteBatch(GraphicsDevice));       
        base.Initialize();
    }


    protected override void LoadContent() {
        Textures.LoadTextures(graphics.GraphicsDevice, Content);

        // Ship must be created after loading textures
        ship = new Ship();

    

        if (LOAD_TRACK && File.Exists("testtrack.dft") ){
            Console.WriteLine("Loading Track!");
            byte[] trackData = File.ReadAllBytes("testtrack.dft");
            track = TrackSerializer.Deserialize(trackData);
        } else {
            Console.WriteLine("Generating track!");
            // Generate track (textures must be loaded before generating)
            var seed = TRACK_SEED;
            if (RANDOM_TRACK) {
                Random rand = new Random();
                seed = rand.Next(0, 999999);
            }
            track = Generator.GenerateTrack(seed);
            if (SAVE_TRACK) {
                File.WriteAllBytes("testtrack.dft", TrackSerializer.Serialize(track));
            }
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

        if ( Keys.Space.IsHeld()) {
            double rotation = ship.Rotation + Math.PI * 1.5;
            ship.AccelerationY = (float) (Math.Sin(rotation) * 0.05);
            ship.AccelerationX = (float) (Math.Cos(rotation) * 0.05);
        }
        else {
            ship.AccelerationY = 0;
            ship.AccelerationX = 0;
        }

        if (shipCollision) {
            ship.X = 0;
            ship.Y = 0;
            ship.ResetMovement();
        }
        else {
            mover.Move(ship);
            camera.X = ship.X;
            camera.Y = ship.Y;
        }

        


        shipCollision = false;
        // Check ship collision
        track.ForBlocksInRange((int) ship.X-20, (int) ship.Y-20, (int) ship.X+20, (int) ship.Y+20, (block, x, y) => {
            Space space = new Space(x, y);
            if (block.Type == BlockType.BORDER)
                if (space.CollidesWith(ship))
                    shipCollision = true;
        });

        

        

        // Update zoom
        camera.Zoom += InputController.MouseWheelDiff() * ZOOM_FACTOR;
        if (camera.Zoom < ZOOM_MIN) camera.Zoom = ZOOM_MIN;       
        if (camera.Zoom > ZOOM_MAX) camera.Zoom = ZOOM_MAX;       

        base.Update(gameTime);
    }


    bool shipCollision = false;


    protected override void Draw(GameTime gameTime) {
        GraphicsDevice.Clear(Color.Black);

        track.ForBlocksInRange((int)(-100 + camera.X), (int)(-100 + camera.Y), (int)(100 + camera.X), (int)(100 + camera.Y), (block, x, y) => {
            //Space space = new Space(0, 0);

            if( block == null )
                Console.WriteLine("Block is null!");
            block.Col = Color.White; //block.Type == BlockType.SPACE ? Color.White : Color.Orange;

            renderer.Draw(camera, block );
        });

        renderer.Draw(camera, ship);

        if (shipCollision)
            ship.Col = Color.Green;
        else
            ship.Col = Color.Red;

        //foreach (CollisionPoint point in CollisionPoint.GetCollisionPoints(ship)) {
        //    renderer.Draw(camera, point);
        //}

        renderer.Flush();
      
        base.Draw(gameTime);
    }

}