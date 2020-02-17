

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

public class Controller : Game {

    private const int MAP_SIZE = 100;
    private const int CAMERA_MOVE_SPEED = 10;
    private const int CELL_SIZE = 10;

    private GraphicsDeviceManager graphics;
    private SpriteBatch spriteBatch;

    private Vector2 camera = new Vector2(0, 0);
    private Field[,] map = new Field[MAP_SIZE, MAP_SIZE];

    private Texture2D tex_Wall;


    public Controller() {
        graphics = new GraphicsDeviceManager(this);
    }


    protected override void LoadContent() {

        // Create a new SpriteBatch, which can be used to draw textures.
        spriteBatch = new SpriteBatch(GraphicsDevice);

        tex_Wall = new Texture2D(graphics.GraphicsDevice, CELL_SIZE, CELL_SIZE);

        Color[] data = new Color[CELL_SIZE * CELL_SIZE];
        for (int i = 0; i < data.Length; ++i)
            data[i] = new Color(75, 75, 75);
        tex_Wall.SetData(data);

        Random rand = new Random();
        for (int x = 0; x < MAP_SIZE; x++) {
            for (int y = 0; y < MAP_SIZE; y++) {
                if (rand.Next(0, 10) < 3)
                    map[y, x] = Field.WALL;
            }
        }
    }




    protected override void Update(GameTime gameTime) {


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

        base.Update(gameTime);
    }



    protected override void Draw(GameTime time) {

        GraphicsDevice.Clear(Color.White);

        spriteBatch.Begin();
        Vector2 pos = new Vector2(0, 0);
        for (int y = 0; y < MAP_SIZE; y++) {
            for (int x = 0; x < MAP_SIZE; x++) {
                if (map[x, y] == Field.WALL)
                    spriteBatch.Draw(tex_Wall, pos - camera, Color.White);
                pos.X += CELL_SIZE;
            }
            pos.Y += CELL_SIZE;
            pos.X = 0;
        }
        spriteBatch.End();

        base.Draw(time);
    }
}


enum Field {
    EMPTY,
    WALL
}