

using DeepFlight.rendering;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;


// Responsible for rendering stuff (sort of a Camera controller)
public class Renderer {

    private SpriteBatch spriteBatch;
    private SpriteBatch trackBatch;
    private bool drawing = false;
    private GraphicsDeviceManager graphics;


    public Renderer(GraphicsDeviceManager graphics) {
        this.graphics = graphics;
        spriteBatch = new SpriteBatch(graphics.GraphicsDevice);
        trackBatch = new SpriteBatch(graphics.GraphicsDevice);
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
            graphics.GraphicsDevice.Clear(Settings.CLEAR_COLOR);

            spriteBatch.Begin(
                sortMode: SpriteSortMode.BackToFront,
                blendState: BlendState.NonPremultiplied,
                samplerState: SamplerState.PointClamp // Turn of anti-alias
            );
        }
    }


    Entity transformed = new Entity();
    public void Draw(Camera camera, TextureView drawable) {
        if (drawable == null)
            throw new ArgumentNullException("TextureView is null");

        if (camera == null)
            throw new ArgumentNullException(string.Format("Camera is null when drawing {0}", drawable.GetType().Name));

        InitializeDraw();

        // Transform the drawables dimensions and coordinates to camera space
        transformed.Inherit(drawable);
        camera.Transform(
            transformed,
            true
        );
        transformed.HOrigin = drawable.HOrigin;
        transformed.VOrigin = drawable.VOrigin;

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
               new Rectangle((int)transformed.GetCenterX(), (int)transformed.GetCenterY(), (int)transformed.Width+1, (int)transformed.Height+1),
               null,
               drawable.Color,
               transformed.Rotation,
               new Vector2(drawable.Texture.Width / 2, drawable.Texture.Height / 2),
               SpriteEffects.None, camera.Layer+drawable.DepthOffset);
    }


    static bool first = true;

    // TEXT
    public void Draw(Camera camera, TextView drawable) {
        if (drawable == null)
            throw new ArgumentNullException("TextView is null");

        if (camera == null)
            throw new ArgumentNullException(string.Format("Camera is null when drawing {0}", drawable.GetType().Name));

        InitializeDraw();

        // Transform the drawables dimensions and coordinates to camera space
        Entity transformed = new Entity(drawable);
        camera.Transform(
            transformed,
            false
        );

        // Scale to screen resolution
        var screenScale = ScreenController.ScreenScale;
        transformed.X *= screenScale;
        transformed.Y *= screenScale;

        // Set draw origin to center of screen
        transformed.X += graphics.PreferredBackBufferWidth / 2.0;
        transformed.Y += graphics.PreferredBackBufferHeight / 2.0;

        double fontScale = transformed.scale * screenScale;
        transformed.Width *= (float)fontScale;
        transformed.Height *= (float)fontScale;

        // Find best SpriteFont for size
        // Since the text has been scaled, best Font size might have
        // changed, so we check and see if a better SpriteFont exists 
        double scaledFontSize = drawable.Size * fontScale;
        FontData bestFont = drawable.Font.GetBestFont(scaledFontSize);
        var bestFontDimensions = bestFont.Sprite.MeasureString(drawable.Text);

        // Calculate the scaling error
        double expectedWidth = drawable.Width * fontScale;
        double scalingError = expectedWidth / bestFontDimensions.X;


        //// TODO: REmove this
        //if (first) {
        //    Console.WriteLine("\n\nDrawing Text!");
        //    Console.WriteLine("Font scale: "  + fontScale);
        //    Console.WriteLine("Scaled size: " + scaledSize);
        //    Console.WriteLine("Best font size: " + bestFont.Size);
        //    Console.WriteLine("Expected width: " + expectedWidth);
        //    Console.WriteLine("Best Font width: " + bestFontWidth);
        //    Console.WriteLine("Scaling error: "  + widthScalingError);
        //    Console.WriteLine("Final scale: " + finalScale);
        //    Console.WriteLine("Base: " + drawable);
        //    Console.WriteLine("Transformed: " + transformed);
        //}

        // Draw the text 
        spriteBatch.DrawString(
            bestFont.Sprite,
            drawable.Text,

            // The drawing position
            // The origin is upper left corner, but it may be adjusted using the
            // offset below (we adjust it to center of texture)
            new Vector2( (float) transformed.GetCenterX(), (float) transformed.GetCenterY() ),

            drawable.Color,
            transformed.Rotation,

            // Offet of drawing (and rotation) origin
            // Note: it's prior scaling (which is why we use the drawn font dimensions)
            new Vector2( (float) (bestFontDimensions.X / 2), (float) (bestFontDimensions.Y/2)),

            // Scaling
            // Scales dimensions of drawn font
            (float) scalingError,
            SpriteEffects.None,


            camera.Layer+drawable.DepthOffset // Layer depth Not sure what this means
        );

        // TODO: Remove this
        first = false;
    }


    public int DrawTrack(Camera camera, Track track) {

        InitializeDraw();

        var transformation = 
           Matrix.CreateTranslation((float) -camera.X, (float) -camera.Y, 0) *
           Matrix.CreateScale((float) ScreenController.ScreenScale) *
           Matrix.CreateScale(camera.Zoom) *
           Matrix.CreateRotationZ(camera.Rotation) *
           Matrix.CreateTranslation(graphics.PreferredBackBufferWidth/2, graphics.PreferredBackBufferHeight / 2, 0);

        // TODO: Test if these can be left out
        trackBatch.Begin(
               //blendState: BlendState.NonPremultiplied,
               //samplerState: SamplerState.PointClamp, // Turn of anti-alias
               transformMatrix: transformation,
               sortMode: SpriteSortMode.BackToFront
        );

        var textureCenterOffset = new Vector2(Textures.SQUARE.Width / 2, Textures.SQUARE.Height / 2);

        trackBatch.Draw(
                Textures.SQUARE,
                // NOTE: Using rectangle to define drawing position (drawing bounds),
                // it will draw from the center of the bounds.
                destinationRectangle: new Rectangle((int) camera.X, (int) camera.Y, graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight),
                color: (track.Planet.Color * Settings.TRACK_COLOR_ADJUST_WALL),
                origin: textureCenterOffset
        );

        var blockCount = 0;

        var chunkMap = track.ChunkMap;

        var renderRadius = Settings.TRACK_RENDER_DISTANCE / 2;
        
        int minX = Chunk.ToChunkCoordinate((int) camera.X - renderRadius);
        int maxX = Chunk.ToChunkCoordinate((int) camera.X + renderRadius);
        int minY = Chunk.ToChunkCoordinate((int) camera.Y - renderRadius);
        int maxY = Chunk.ToChunkCoordinate((int) camera.Y + renderRadius);


        for (int x = minX; x <= maxX; x++) {
            if (chunkMap.ContainsKey(x)) {
                var row = chunkMap[x];
                for (int y = minY; y <= maxY; y++) {
                    if (row.ContainsKey(y)) {
                        var blocks = row[y].Blocks;
                        for (int i = 0; i < blocks.Length; i++) {
                            trackBatch.Draw(
                                   Textures.SQUARE,
                                   // NOTE: Using rectangle to define drawing position (drawing bounds),
                                   // it will draw from the center of the bounds.
                                   destinationRectangle: new Rectangle(blocks[i].x, blocks[i].y, 1, 1),
                                   color: Color.White,
                                   origin: textureCenterOffset
                            );
                            blockCount++;
                        }
                    }
                }
            }
        }

        trackBatch.End();
        return blockCount;
    }


    // Flush Renderer draw batch
    public void Flush() {
        if( drawing ) {
            drawing = false;
            spriteBatch.End();
        }
    }
}