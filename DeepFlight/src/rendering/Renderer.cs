

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;


// Responsible for rendering stuff (sort of a Camera controller)
public class Renderer {

    private SpriteBatch spriteBatch;
    private bool drawing = false;
    private GraphicsDeviceManager graphics;


    public Renderer(GraphicsDeviceManager graphics) {
        this.graphics = graphics;

        graphics.PreferredBackBufferWidth = 1080;
        graphics.PreferredBackBufferHeight = 720;

        spriteBatch = new SpriteBatch(graphics.GraphicsDevice);
    }

    public Vector2 GetScreenSize() {
        return new Vector2(graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight);
    }


    private void InitializeDraw() {
        // Initialize Renderer draw batch
        if (!drawing) {
            if (spriteBatch == null)
                throw new NullReferenceException("Sprite batch is null");
            drawing = true;
            graphics.GraphicsDevice.Clear(Color.Black);
            spriteBatch.Begin();
        }
    }


    public void DrawTexture(Camera camera, DrawableTexture drawable) {
        InitializeDraw();

        // Transform the drawables dimensions and coordinates to camera space
        Entity transformed = camera.Transform(
            drawable,
            graphics.PreferredBackBufferWidth,
            graphics.PreferredBackBufferHeight
        );
        //Entity transformed = drawable;

        // Draw the texture
        spriteBatch.Draw(
               drawable.Texture,
               new Rectangle((int)transformed.X, (int)transformed.Y, (int)transformed.Width, (int)transformed.Height),
               null,
               drawable.Col,
               transformed.Rotation,
               new Vector2(drawable.Texture.Width / 2, drawable.Texture.Height / 2),
               SpriteEffects.None, 0f);

        if( first ) {
            first = false;
            Console.WriteLine("Transformed: " + transformed);
        }
    }

    bool first = true;

    public void DrawText(Camera camera, DrawableText drawable) {
        InitializeDraw();

        // Transform the drawables dimensions and coordinates to camera space
        Entity transformed = camera.Transform(
            drawable,
            graphics.PreferredBackBufferWidth,
            graphics.PreferredBackBufferHeight
        );

        spriteBatch.DrawString(
            drawable.Font,
            drawable.Text,
            new Vector2((int)transformed.X, (int)transformed.Y),
            drawable.Col,
            transformed.Rotation,
            new Vector2(drawable.Width / 2, drawable.Height / 2), // Center of rotation
            transformed.scale,
            SpriteEffects.None,
            0f // Layer depth Not sure what this means
        );

        if (first) {
            first = false;
            Console.WriteLine("Transformed: " + transformed);
        }


        //spriteBatch.DrawString(
        //    drawable.Font,
        //    drawable.Text,
        //    new Vector2((int)transformed.X, (int)transformed.Y),
        //    drawable.Col
        //);
    }

    // Flush Renderer draw batch
    public void Flush() {
        if( drawing ) {
            drawing = false;
            spriteBatch.End();
        }
    }
}