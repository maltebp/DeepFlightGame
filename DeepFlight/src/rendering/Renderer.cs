

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


    public void Draw(Camera camera, Drawable drawable) {

        // Initialize Renderer draw batch
        if (!drawing) {
            drawing = true;
            graphics.GraphicsDevice.Clear(Color.Black);
            spriteBatch.Begin();
        }


        // Scale the object to camera
        double drawX = drawable.X * camera.Zoom;
        double drawY = drawable.Y * camera.Zoom;

        // Adjust to camera position
        drawX += (camera.Width / 2 - camera.X*camera.Zoom);
        drawY += (camera.Height / 2 - camera.Y*camera.Zoom);

        // Adjust to center of entity
        drawX -= drawable.Width / 2;
        drawY -= drawable.Height / 2;
        double centerX = camera.Width / 2;
        double centerY = camera.Height / 2;

        // Rotate around camera center
        double rotatedX = Math.Cos(-camera.Rotation) * (drawX - centerX) - Math.Sin(-camera.Rotation) * (drawY - centerY) + centerX;
        double rotatedY = Math.Sin(-camera.Rotation) * (drawX - centerX) + Math.Cos(-camera.Rotation) * (drawY - centerY) + centerY;

        if (spriteBatch == null)
            throw new NullReferenceException("Sprite batch is null");

         spriteBatch.Draw(
            drawable.Texture,
            new Rectangle( (int) rotatedX, (int) rotatedY, (int) (drawable.Width*camera.Zoom*drawable.scale+2), (int) (drawable.Height*camera.Zoom*drawable.scale+2)),
            null,
            drawable.Col,
            drawable.Rotation - camera.Rotation,
            new Vector2(drawable.Texture.Width/2, drawable.Texture.Height/2),
            SpriteEffects.None, 0f);
    }


    // Flush Renderer draw batch
    public void Flush() {
        drawing = false;
        spriteBatch.End();
    }
}