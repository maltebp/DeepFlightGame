

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

public class Controller : Game {



    private const int CAMERA_MOVE_SPEED = 10;
    private const int GENERATION_DISTANCE = 2;

    private GraphicsDeviceManager graphics;
    private SpriteBatch spriteBatch;
    private SpriteFont font;

    private Vector2 camera = new Vector2(0, 0);

    private Texture2D tex_Line;
    private Texture2D tex_Wall;
    private Texture2D tex_Circle;

    // FPS
    private double FPS_UPDATE_FREQ = 0.5;
    private double fpsUpdateCounter = 0;
    private double currentFps = -1;
    private FPSCounter fpsCounter = new FPSCounter();

    private Random random = new Random();
    

    public Controller() {
        graphics = new GraphicsDeviceManager(this);
        graphics.PreferredBackBufferWidth = 1080;  // set this value to the desired width of your window
        graphics.PreferredBackBufferHeight = 720;   // set this value to the desired height of your window
        graphics.ApplyChanges();
    }


    protected override void Initialize() {
        

        base.Initialize();
    }


    protected override void LoadContent() {



        // Create a new SpriteBatch, which can be used to draw textures.
        spriteBatch = new SpriteBatch(GraphicsDevice);

        // Wall Texture
        tex_Wall = new Texture2D(graphics.GraphicsDevice, Generator.CELL_SIZE, Generator.CELL_SIZE);

        Color[] data = new Color[Generator.CELL_SIZE * Generator.CELL_SIZE];
        for (int i = 0; i < data.Length; ++i)
            data[i] = new Color(75, 75, 75);
        tex_Wall.SetData(data);

        // Line Texture
        tex_Line = new Texture2D(graphics.GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
        tex_Line.SetData(new[] { Color.White });

        // Circle Texture
        tex_Circle = Content.Load<Texture2D>("Content/CircleTexture2");

        // Font
        font = Content.Load<SpriteFont>("Content/DefaultFont");        

    }


    KeyboardState newState;
    KeyboardState oldState;

    protected override void Update(GameTime gameTime) {
        newState = Keyboard.GetState();

        fpsCounter.Update(gameTime);        
    

        if (Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();
        if (Keyboard.GetState().IsKeyDown(Keys.Left))
            camera.X -= CAMERA_MOVE_SPEED;
        if (Keyboard.GetState().IsKeyDown(Keys.Right))
            camera.X += CAMERA_MOVE_SPEED;
        if (Keyboard.GetState().IsKeyDown(Keys.Up))
            camera.Y -= CAMERA_MOVE_SPEED;
        if (Keyboard.GetState().IsKeyDown(Keys.Down))
            camera.Y += CAMERA_MOVE_SPEED;

        Generator.GenerateSector(camera, GENERATION_DISTANCE);

        if (KeypressTest(Keys.R)) {
            Console.WriteLine("\n\nSECTOR (Size: {0})", Generator.GetSector());
            foreach (Block block in Generator.GetSector()) {
                Console.WriteLine("Block ({0}, {1}):", block.getX(), block.getY());
                foreach (Node node in block.GetNodes()) {
                    Console.WriteLine("\t{0}", node.GetPos());
                }
            }
        }
        
        
  
        base.Update(gameTime);
        oldState = newState;
    }

    //private void generate() {
    //    int minBlockX = Generator.GetBlockX((int) camera.X);
    //    int minBlockY = Generator.GetBlockY((int) camera.Y);
    //    int maxBlockX = Generator.GetBlockX()

    //    ulong i = (ulong) random.Next();
    //    Generator.setMasterSeed(i*i);
    //    nodes = Generator.generateBlock(-100, -100, 100, 100);
    //}



    protected override void Draw(GameTime time) {

        fpsCounter.Update(time);
        fpsUpdateCounter += time.ElapsedGameTime.TotalSeconds;
        if (fpsUpdateCounter > FPS_UPDATE_FREQ) {
            currentFps = fpsCounter.GetFPS();
            fpsUpdateCounter %= FPS_UPDATE_FREQ;
        }

        GraphicsDevice.Clear(Color.White);

        spriteBatch.Begin();
        //Vector2 pos = new Vector2(0, 0);
        //for (int y = 0; y < MAP_SIZE; y++) {
        //    for (int x = 0; x < MAP_SIZE; x++) {
        //        if (map[x, y] == Field.WALL)
        //            spriteBatch.Draw(tex_Wall, pos - camera, Color.White);
        //        pos.X += CELL_SIZE;
        //    }
        //    pos.Y += CELL_SIZE;
        //    pos.X = 0;
        //}

        //DrawLine(spriteBatch, new Vector2(0,0), new Vector2(500,500), Color.Red, 10);

        foreach (Block block in Generator.GetSector()) {
            foreach (Node node in block.GetNodes() ) {
                Vector2 drawPos = node.GetPos() - camera;
                spriteBatch.Draw(tex_Circle, new Rectangle((int)drawPos.X, (int)drawPos.Y, Generator.CELL_SIZE, Generator.CELL_SIZE), Color.Black);
            }
        }

        spriteBatch.DrawString(font, String.Format("Cam: {0}, {1}", camera.X, camera.Y), new Vector2(50, 50), Color.Red );
        spriteBatch.DrawString(font, String.Format("Block: {0}, {1}", Generator.GetBlockX((int) camera.X), Generator.GetBlockY((int) camera.Y)), new Vector2(50, 100), Color.Red);
        spriteBatch.DrawString(font, String.Format("N. Blocks: {0}", Generator.GetSector().Count), new Vector2(50, 150), Color.Red);


        spriteBatch.DrawString(font, String.Format("FPS: {0:N2}", currentFps ), new Vector2(50, 200), Color.Red);

        spriteBatch.End();
        base.Draw(time);
    }

    //public void UpdateBlocks() {
    //    int currentBlockX = camera.X 
    //}


    // Line source: http://community.monogame.net/t/line-drawing/6962/4
    public void DrawLine( SpriteBatch spriteBatch, Vector2 point1, Vector2 point2, Color color, float thickness = 1f) {
        var distance = Vector2.Distance(point1, point2);
        var angle = (float)Math.Atan2(point2.Y - point1.Y, point2.X - point1.X);
        DrawLine(spriteBatch, point1, distance, angle, color, thickness);
    }

    public void DrawLine( SpriteBatch spriteBatch, Vector2 point, float length, float angle, Color color, float thickness = 1f) {
        var origin = new Vector2(0f, 0.5f);
        var scale = new Vector2(length, thickness);
        spriteBatch.Draw( tex_Line, point, null, color, angle, origin, scale, SpriteEffects.None, 0);
    }


    private bool KeypressTest(Keys theKey) {
        if (newState.IsKeyUp(theKey) && oldState.IsKeyDown(theKey))
            return true;
        return false;
    }
}






enum Field {
    EMPTY,
    WALL
}