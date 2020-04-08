


using DeepFlight.rendering;
using DeepFlight.src.scenes;
using DeepFlight.utility.KeyboardController;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;

namespace DeepFlight.scenes {
    
    public class GameScene : Scene {

        private static float DEFAULT_ZOOM = 7f;

        private Camera gameCamera = new Camera();
        private Camera uiCamera = new Camera();

        // Number of track blocks to render in a radius
        public int renderDistance = 100;

        private Track track;
        private Ship ship;

        public GameScene(Track track) {
            this.track = track;
            gameCamera.Zoom = DEFAULT_ZOOM;
        }



        protected override void OnInitialize() {



            // Height of UI screen
            float height = (float) ScreenController.BaseHeight;
            float width = (float) ScreenController.BaseWidth;

            ship = new Ship();

        }






        protected override bool OnKeyInput(KeyEventArgs e) {
            if (e.Action == KeyAction.PRESSED) {

                // Quit track
                if (e.Key == Keys.Escape) {
                    RequestSceneSwitch(new TrackSelectionScene());
                    return true;
                }
            }

            if( e.Action == KeyAction.HELD ) {
                if( e.Key == Keys.Left) {
                    ship.Rotation -= 0.05f;
                    return true;
                }
                if (e.Key == Keys.Right) {
                    ship.Rotation += 0.05f;
                    return true;
                }
                 
                if (e.Key == Keys.Space) {
                    double rotation = ship.Rotation + Math.PI * 1.5;
                    ship.AccelerationY = (float)(Math.Sin(rotation) * 0.05);
                    ship.AccelerationX = (float)(Math.Cos(rotation) * 0.05);
                }
            }

            if( e.Action == KeyAction.RELEASED) {

                if( e.Key == Keys.Space) {
                    ship.AccelerationX = 0;
                    ship.AccelerationY = 0;
                }
            }




            //        if (Keys.Left.IsHeld()) {
            //            ship.Rotation -= 0.05f;
            //        }

            //        if (Keys.Right.IsHeld()) {
            //            ship.Rotation += 0.05f;
            //        }

            //        if ( Keys.Space.IsHeld()) {
            //            double rotation = ship.Rotation + Math.PI * 1.5;
            //            ship.AccelerationY = (float) (Math.Sin(rotation) * 0.05);
            //            ship.AccelerationX = (float) (Math.Cos(rotation) * 0.05);
            //        }
            //        else {
            //            ship.AccelerationY = 0;
            //            ship.AccelerationX = 0;
            //        }

            //        if (shipCollision) {
            //            ship.X = 0;
            //            ship.Y = 0;
            //            ship.ResetMovement();
            //        }
            //        else {
            //            mover.Move(ship);
            //            camera.X = ship.X;
            //            camera.Y = ship.Y;
            //        }

            return false;
        }

        protected override void OnUpdate(double deltaTime) {
            //if (shipCollision) {
            //    ship.X = 0;
            //    ship.Y = 0;
            //    ship.ResetMovement();
            //}
            //else {
            //    mover.Move(ship);

            //}

            Mover.Move(ship);
            gameCamera.X = ship.X;
            gameCamera.Y = ship.Y;
        }



        protected override void OnDraw(Renderer renderer) {

            track.ForBlocksInRange((int)(-100 + gameCamera.X), (int)(-100 + gameCamera.Y), (int)(100 + gameCamera.X), (int)(100 + gameCamera.Y), (block, x, y) => {
                //Space space = new Space(0, 0);

                if (block == null)
                    Console.WriteLine("Block is null!");

                //block.Col = Color.White; //block.Type == BlockType.SPACE ? Color.White : Color.Orange;

                renderer.Draw(gameCamera, block);
            });

            renderer.Draw(gameCamera, ship);

            //if (shipCollision)
            //    ship.Col = Color.Green;
            //else
            //    ship.Col = Color.Red;
                
            //foreach (CollisionPoint point in CollisionPoint.GetCollisionPoints(ship)) {
            //    renderer.Draw(camera, point);
            //}
        }


    }

}