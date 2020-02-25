

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

public class Controller : Game {

    private const double SCALE = 5;
    private const int CAMERA_MOVE_SPEED = 5;
    private const int GENERATION_DISTANCE = 2;
    private float rotation = 0.0f;
    private float ROTATION_SPEED = 0.1f;

    private GraphicsDeviceManager graphics;
    private SpriteBatch spriteBatch;
    private SpriteFont font;

    private int CAM_WIDTH = 1920;
    private int CAM_HEIGHT = 1080;

    private Vector2 camera = new Vector2(0, 0);

    private Texture2D tex_Line;
    private Texture2D tex_Wall;
    private Texture2D tex_Circle;

    // FPS
    private double FPS_UPDATE_FREQ = 0.5;
    private double fpsUpdateCounter = 0;
    private double currentFps = -1;

    private double simUpdateCounter = 0;
    private double currentSim = -1;

    private FPSCounter fpsCounter = new FPSCounter();
    private FPSCounter simCounter = new FPSCounter();

    private Random random = new Random();
    private Generator generator = new Generator();


    LinkedList<Vector2> wallsToDraw = new LinkedList<Vector2>();

    public Controller() {
        graphics = new GraphicsDeviceManager(this);
        graphics.PreferredBackBufferWidth = CAM_WIDTH;  // set this value to the desired width of your window
        graphics.PreferredBackBufferHeight = CAM_HEIGHT;   // set this value to the desired height of your window
        graphics.ApplyChanges();
    }


    protected override void Initialize() {

        generator.GenerateTrack();

        //foreach(Node node in path.GetNodes()) {
        //    Console.WriteLine(node);
        //}


        base.Initialize();
    }


    protected override void LoadContent() {



        // Create a new SpriteBatch, which can be used to draw textures.
        spriteBatch = new SpriteBatch(GraphicsDevice);

        tex_Wall = new Texture2D(graphics.GraphicsDevice, 10, 10);

        Color[] data = new Color[10 * 10];
        for (int i = 0; i < data.Length; ++i)
            data[i] = new Color(255, 255, 255);
        tex_Wall.SetData(data);

        // Line Texture
        //tex_Line = new Texture2D(graphics.GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
        //tex_Line.SetData(new[] { Color.White });

        // Circle Texture
        tex_Circle = Content.Load<Texture2D>("Content/CircleTexture2");

        // Font
        font = Content.Load<SpriteFont>("Content/DefaultFont");


    }


    KeyboardState newState;
    KeyboardState oldState;

    protected override void Update(GameTime gameTime) {
        newState = Keyboard.GetState();

        simCounter.Update(gameTime);


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

        if (Keyboard.GetState().IsKeyDown(Keys.W))
            rotation = (rotation - ROTATION_SPEED) % (float)(Math.PI * 2);
        if (Keyboard.GetState().IsKeyDown(Keys.E))
            rotation = (rotation+ROTATION_SPEED) % (float) (Math.PI*2);



        //Generator.GenerateSector(camera, GENERATION_DISTANCE);

        //if (KeypressTest(Keys.R)) {
        //    Console.WriteLine("\n\nSECTOR (Size: {0})", Generator.GetSector());
        //    foreach (Block block in Generator.GetSector()) {
        //        Console.WriteLine("Block ({0}, {1}):", block.getX(), block.getY());
        //        foreach (Node node in block.GetNodes()) {
        //            Console.WriteLine("\t{0}", node.GetPos());
        //        }
        //    }
        //}

        generator.UpdateOffset((int) camera.X, (int) camera.Y);

        wallsToDraw.Clear();
        

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

        simUpdateCounter += time.ElapsedGameTime.TotalSeconds;
        if (simUpdateCounter > FPS_UPDATE_FREQ) {
            currentSim = simCounter.GetFPS();
            simUpdateCounter %= FPS_UPDATE_FREQ;
        }

        GraphicsDevice.Clear(Color.Black);

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

        //foreach (Block block in Generator.GetSector()) {
        //    foreach (Node node in block.GetNodes()) {
        //        Vector2 drawPos = node.GetPos() - camera;
        //        spriteBatch.Draw(tex_Circle, new Rectangle((int)drawPos.X, (int)drawPos.Y, Generator.CELL_SIZE, Generator.CELL_SIZE), Color.Black);
        //    }
        //}

//        p'x = cos(theta) * (px-ox) - sin(theta) * (py-oy) + ox

//p'y       =   sin(theta) * (px-ox) + cos(theta) * (py-oy) + oy

        int size = (int)SCALE;
        int adjust = (int)SCALE / 2;
        double centerX = CAM_WIDTH / 2;
        double centerY = CAM_HEIGHT / 2;

        generator.ForEachBlock((block, x, y) => {
            if (block == BlockType.WALL) {
                double drawX = (x - camera.X) * SCALE + CAM_WIDTH / 2;// - (CAM_WIDTH*SCALE)/2;
                double drawY = (y - camera.Y) * SCALE + CAM_HEIGHT / 2;

                double rotatedX = Math.Cos(rotation) * (drawX - centerX) - Math.Sin(rotation) * (drawY - centerY) + centerX;
                double rotatedY = Math.Sin(rotation) * (drawX - centerX) + Math.Cos(rotation) * (drawY - centerY) + centerY;

                if (rotatedX >= 0 && rotatedX < CAM_WIDTH && rotatedY >= 0 && rotatedY < CAM_HEIGHT)
                    //spriteBatch.Draw(cannon.image, new Rectangle(300, 300, cannon.image.Width, cannon.image.Height), null, Color.White, y, origin, SpriteEffects.None, 0f);

                    //spriteBatch.Draw(tex_Wall, new Rectangle((int)(drawX - adjust), (int)(drawY - adjust), size, size), null, Color.White, rotation, new Vector2((float) drawX, (float) drawY), SpriteEffects.None, 0f );
                    spriteBatch.Draw(tex_Wall, new Rectangle((int)(rotatedX - adjust), (int)(rotatedY - adjust), size, size), Color.White);
            }
        });



        //foreach( Node node in path.GetNodes()) {
        //    double x = node.x - camera.X;
        //    double y = node.y - camera.Y;
        //    spriteBatch.Draw(tex_Circle, new Rectangle( (int) x-5, (int) y-5, 10, 10), Color.Black);
        //}

        spriteBatch.DrawString(font, String.Format("Cam: {0}, {1}", camera.X, camera.Y), new Vector2(50, 50), Color.Red);
        spriteBatch.DrawString(font, String.Format("Rotation.: {0:N2}", rotation), new Vector2(50, 80), Color.Red);

        //spriteBatch.DrawString(font, String.Format("Block: {0}, {1}", Generator.GetBlockX((int)camera.X), Generator.GetBlockY((int)camera.Y)), new Vector2(50, 100), Color.Red);
        //spriteBatch.DrawString(font, String.Format("N. Blocks: {0}", Generator.GetSector().Count), new Vector2(50, 150), Color.Red);

        spriteBatch.DrawString(font, String.Format("FPS: {0:N2}", currentFps), new Vector2(50, 200), Color.Red);
        spriteBatch.DrawString(font, String.Format("Sim.: {0:N2}", currentSim), new Vector2(50, 250), Color.Red);


        spriteBatch.End();
        base.Draw(time);
    }

    //public void UpdateBlocks() {
    //    int currentBlockX = camera.X 
    //}


    // Line source: http://community.monogame.net/t/line-drawing/6962/4
    public void DrawLine(SpriteBatch spriteBatch, Vector2 point1, Vector2 point2, Color color, float thickness = 1f) {
        var distance = Vector2.Distance(point1, point2);
        var angle = (float)Math.Atan2(point2.Y - point1.Y, point2.X - point1.X);
        DrawLine(spriteBatch, point1, distance, angle, color, thickness);
    }

    public void DrawLine(SpriteBatch spriteBatch, Vector2 point, float length, float angle, Color color, float thickness = 1f) {
        var origin = new Vector2(0f, 0.5f);
        var scale = new Vector2(length, thickness);
        spriteBatch.Draw(tex_Line, point, null, color, angle, origin, scale, SpriteEffects.None, 0);
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