﻿

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

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



        double drawX = drawable.X * camera.Zoom;
        double drawY = drawable.Y * camera.Zoom;

        // Adjust to camera position
        drawX += (camera.Width / 2 - camera.X*camera.Zoom);
        drawY += (camera.Height / 2 - camera.Y*camera.Zoom);

     
        double centerX = camera.Width / 2;
        double centerY = camera.Height / 2;

        double rotatedX = Math.Cos(-camera.Rotation) * (drawX - centerX) - Math.Sin(-camera.Rotation) * (drawY - centerY) + centerX;
        double rotatedY = Math.Sin(-camera.Rotation) * (drawX - centerX) + Math.Cos(-camera.Rotation) * (drawY - centerY) + centerY;

        // Adjust to center of entity
        //drawX -= drawable.Width / 2;
        //drawY -= drawable.Height / 2;

        spriteBatch.Draw(
            drawable.Texture,
            new Rectangle( (int) rotatedX, (int) rotatedY, (int) (drawable.Width*camera.Zoom+2), (int) (drawable.Height*camera.Zoom+2)),
            null,
            drawable.Col,
            drawable.Rotation - camera.Rotation,
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