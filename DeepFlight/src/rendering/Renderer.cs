

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;


// Responsible for rendering stuff (sort of a Camera controller)
public class Renderer {

    private static readonly Color CLEAR_COLOR = Color.White;
    private SpriteBatch spriteBatch;
    private bool drawing = false;
    private GraphicsDeviceManager graphics;

    private int screenWidth;
    private int screenHeight;

    private float screenScale = 0;

    private int LOGICAL_SCREEN_HEIGHT = 500;

    // 1080 / 500 -> 2,.. = the scaling factor
    //


    public Renderer(GraphicsDeviceManager graphics) {
        this.graphics = graphics;

        // Fetch screen dimensions
        screenWidth = graphics.GraphicsDevice.DisplayMode.Width;
        screenHeight = graphics.GraphicsDevice.DisplayMode.Height;

        // Get the actual screen dimensions
        graphics.ToggleFullScreen();

        SetResolution(1920, 1080);            

        Console.WriteLine("Screen width: " + screenWidth);
        Console.WriteLine("Sceren height: " + screenHeight);

        spriteBatch = new SpriteBatch(graphics.GraphicsDevice);
    }


    public Vector2 GetScreenSize() {
        return new Vector2(graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight);
    }


    public void SetResolution(int width, int height) {
        Console.WriteLine("Setting resolution: {0}x{1}", width, height);
        graphics.PreferredBackBufferWidth = width;
        graphics.PreferredBackBufferHeight = height;
        graphics.ApplyChanges();

        //float scaleX = graphics.PreferredBackBufferHeight / (float) screenHeight;// graphics.PreferredBackBufferWidth / screenWidth;
        //float scaleY = scaleX;// graphics.PreferredBackBufferHeight / screenHeight;

        Console.WriteLine("Screen height: " + screenHeight);
        screenScale = graphics.PreferredBackBufferHeight / (float) LOGICAL_SCREEN_HEIGHT;
        Console.WriteLine("Scaling: " + screenScale);
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
        transformed.X *= screenScale;
        transformed.Y *= screenScale;
        transformed.Width *= screenScale;
        transformed.Height *= screenScale;

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
        transformed.X *= screenScale;
        transformed.Y *= screenScale;

        // Set draw origin to center of screen
        transformed.X += graphics.PreferredBackBufferWidth / 2;
        transformed.Y += graphics.PreferredBackBufferHeight / 2;

        // Find best SpriteFont for size
        // Since the text has been scaled, best Font size might have
        // changed, so we check and see if a better SpriteFont exists 
        float fontScale = transformed.scale * (screenScale/2);
        float scaledSize = drawable.Size * fontScale;
        int bestSize = 0;
        float bestDiff = 100000;
        foreach( var entry in drawable.Font.GetFontMap()) {
            var fontSize = entry.Key;
            var sizeDiff = Math.Abs(scaledSize - fontSize);
            if ( sizeDiff < bestDiff) {
                bestDiff = sizeDiff;
                bestSize = fontSize;
            }
        }
        float finalScale = scaledSize / bestSize;
        transformed.Size = bestSize;

        // Draw the text 
        spriteBatch.DrawString(
            drawable.Font.GetFont(bestSize),
            drawable.Text,

            // Apparently, the origin affects the drawing position as well (unlike with textures),
            // so we have to adjust this transformed position with the same amount as the origin.
            new Vector2((int) transformed.GetCenterX(), (int) transformed.GetCenterY() ),

            drawable.Col,
            transformed.Rotation,

            // Adjusts the drawing origin, including rotation. (Doesnn't work quite the same as when drawing textures)
            // Setting it to width and height divided by two, it centers the text (not sure why)
            new Vector2(transformed.Width / 2, transformed.Height / 2),

            // This ensure that text remains same size, regardless of screen resolution
            finalScale,
            SpriteEffects.None,

            0f // Layer depth Not sure what this means
        );
    }


    // Flush Renderer draw batch
    public void Flush() {
        if( drawing ) {
            drawing = false;
            spriteBatch.End();
        }
    }
}