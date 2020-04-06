

using DeepFlight.rendering;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;


// Responsible for rendering stuff (sort of a Camera controller)
public class Renderer {

    private static readonly Color CLEAR_COLOR = Color.Green;
    private SpriteBatch spriteBatch;
    private bool drawing = false;
    private GraphicsDeviceManager graphics;


    public Renderer(GraphicsDeviceManager graphics) {
        this.graphics = graphics;
        spriteBatch = new SpriteBatch(graphics.GraphicsDevice);
    }

    /// <summary>
    /// Initialize a new drawing session, by clearing the screen
    /// and beginning the spriteBatch.
    /// </summary>
    private void InitializeDraw() {
        // Initialize Renderer draw batch
        if (!drawing) {
            if (spriteBatch == null)
                throw new NullReferenceException("Sprite batch is null");
            drawing = true;
            graphics.GraphicsDevice.Clear(CLEAR_COLOR);
            spriteBatch.Begin();
        }
    }


    public void Draw(Camera camera, DrawableTexture drawable) {
        InitializeDraw();

        // Transform the drawables dimensions and coordinates to camera space
        Entity transformed = new Entity(drawable);
        camera.Transform(
            transformed,
            true
        );

        // Scale width and height
        transformed.Width *= transformed.scale;
        transformed.Height *= transformed.scale;

        // Scale to screen resolution
        var screenScale = ScreenController.ScreenScale;
        transformed.X *= screenScale;
        transformed.Y *= screenScale;
        transformed.Width *= (float) screenScale;
        transformed.Height *= (float) screenScale;

        // Set draw origin to center of screen
        transformed.X += graphics.PreferredBackBufferWidth / 2;
        transformed.Y += graphics.PreferredBackBufferHeight / 2;

        // Draw the texture
        spriteBatch.Draw(
               drawable.Texture,
               // NOTE: Using rectangle to define drawing position (drawing bounds),
               // it will draw from the center of the bounds.
               new Rectangle((int)transformed.X, (int)transformed.Y, (int)transformed.Width, (int)transformed.Height),
               null,
               drawable.Col,
               transformed.Rotation,
               new Vector2(drawable.Texture.Width / 2, drawable.Texture.Height / 2),
               SpriteEffects.None, 0f);
    }


    static bool first = true;

    // TEXT
    public void Draw(Camera camera, DrawableText drawable) {
        InitializeDraw();

        // Transform the drawables dimensions and coordinates to camera space
        DrawableText transformed = new DrawableText(drawable.Text, drawable.Font, drawable.Size, drawable.Col, drawable.X, drawable.Y);
        camera.Transform(
            transformed,
            false
        );
        transformed.HOrigin = drawable.HOrigin;
        transformed.VOrigin = drawable.VOrigin;

        // Scale to screen resolution
        var screenScale = ScreenController.ScreenScale;
        transformed.X *= screenScale;
        transformed.Y *= screenScale;

        // Set draw origin to center of screen
        transformed.X += graphics.PreferredBackBufferWidth / 2.0;
        transformed.Y += graphics.PreferredBackBufferHeight / 2.0;

        // Find best SpriteFont for size
        // Since the text has been scaled, best Font size might have
        // changed, so we check and see if a better SpriteFont exists 
        double fontScale = transformed.scale * screenScale;
        double scaledSize = drawable.Size * fontScale;
        FontData bestFont = drawable.Font.GetBestFont(scaledSize);

        double expectedWidth = drawable.Width * fontScale;
        double widthScalingError = bestFont.Sprite.MeasureString(drawable.Text).X / expectedWidth;
        double correctedSize = bestFont.Size * widthScalingError;

        double finalScale = scaledSize / correctedSize;
        transformed.Size = correctedSize;

        if (first) {
            Console.WriteLine("\n\nDrawing Text!");
            Console.WriteLine("Best Size: " + bestFont.Size);
            Console.WriteLine("Final scale: " + finalScale);
            Console.WriteLine("Base: " + drawable);
            Console.WriteLine("Transformed: " + transformed);
        }

        // Draw the text 
        spriteBatch.DrawString(
            bestFont.Sprite,
            drawable.Text,

            // Apparently, the origin affects the drawing position as well (unlike with textures),
            // so we have to adjust this transformed position with the same amount as the origin.
            new Vector2( (float) transformed.GetCenterX(), (float) transformed.GetCenterY() ),

            drawable.Col,
            transformed.Rotation,

            // Adjusts the drawing origin, including rotation. (Doesnn't work quite the same as when drawing textures)
            // Setting it to width and height divided by two, it centers the text (not sure why)
            new Vector2(transformed.Width / 2, transformed.Height / 2),

            // This ensure that text remains same size, regardless of screen resolution
            (float) finalScale,
            SpriteEffects.None,

            0f // Layer depth Not sure what this means
        );

        first = false;
    }


    // Flush Renderer draw batch
    public void Flush() {
        if( drawing ) {
            drawing = false;
            spriteBatch.End();
        }
    }
}