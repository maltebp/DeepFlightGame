

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

class Renderer {

    private SpriteBatch spriteBatch;
    private bool drawing = false;

    public Renderer(SpriteBatch spriteBatch) {
        this.spriteBatch = spriteBatch;
    }

    public void Draw(Camera camera, Drawable drawable) {

        // Initialize Renderer draw batch
        if (!drawing) {
            drawing = true;
            spriteBatch.Begin();}

        double drawX = drawable.X, drawY = drawable.Y;

        // Adjust to center of entity
        drawX -= drawable.Width / 2;
        drawY -= drawable.Height / 2;

        // Adjust to camera position
        drawX += (camera.Width / 2 - camera.X);
        drawY += (camera.Height / 2 - camera.Y);

        spriteBatch.Draw(
            drawable.Texture,
            new Rectangle( (int) drawX, (int) drawY, (int) drawable.Width, (int) drawable.Height),
            null,
            drawable.Color,
            drawable.Rotation,
            new Vector2(drawable.Texture.Width/2, drawable.Texture.Height/2),
            SpriteEffects.None, 0f);
        
        

        //int size = (int)SCALE;
        //int adjust = (int)SCALE / 2;
        //double centerX = CAM_WIDTH / 2;
        //double centerY = CAM_HEIGHT / 2;

        //generator.ForEachBlock((block, x, y) => {
        //    if (block == BlockType.WALL) {
        //        double drawX = (x - camera.X) * SCALE + CAM_WIDTH / 2;// - (CAM_WIDTH*SCALE)/2;
        //        double drawY = (y - camera.Y) * SCALE + CAM_HEIGHT / 2;

        //        double rotatedX = Math.Cos(rotation) * (drawX - centerX) - Math.Sin(rotation) * (drawY - centerY) + centerX;
        //        double rotatedY = Math.Sin(rotation) * (drawX - centerX) + Math.Cos(rotation) * (drawY - centerY) + centerY;


        //        if (rotatedX >= 0 && rotatedX < CAM_WIDTH && rotatedY >= 0 && rotatedY < CAM_HEIGHT)
        //            //spriteBatch.Draw(cannon.image, new Rectangle(300, 300, cannon.image.Width, cannon.image.Height), null, Color.White, y, origin, SpriteEffects.None, 0f);

        //            spriteBatch.Draw(tex_Wall, new Rectangle((int)(rotatedX - adjust), (int)(rotatedY - adjust), size + 1, size + 1), null, Color.White, rotation, new Vector2(5, 5), SpriteEffects.None, 0f);
        //        //spriteBatch.Draw(tex_Wall, new Rectangle((int)(rotatedX - adjust), (int)(rotatedY - adjust), size, size), Color.White);
        //    }
        //});

    }





    // Flush Renderer draw batch
    public void Flush() {
        drawing = false;
        spriteBatch.End();
    }

    

    


}