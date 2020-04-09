


using DeepFlight.rendering;
using DeepFlight.src.scenes;
using DeepFlight.utility.KeyboardController;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace DeepFlight.scenes {
    
    public class GameScene : Scene {

        private static float DEFAULT_ZOOM = 7f;

        private Camera gameCamera = new Camera();
        private Camera uiCamera = new Camera();

        // Number of track blocks to render in a radius
        public int renderDistance = 100;

        private Track track;
        private Ship ship;

        private TextView timeText;
        private TextureView timeTextBox;
        private TextView countdownText;
        private TextureView countdownTextBox;
        private Stopwatch stopWatch = new Stopwatch();

        private int startCountdown_TickRate = 1000; // ms 

        private bool shipPaused = false;
        

        public GameScene(Track track) {
            this.track = track;
            gameCamera.Zoom = DEFAULT_ZOOM;
        }



        protected override void OnInitialize() {

            // Height of UI screen
            float height = (float) ScreenController.BaseHeight;
            float width = (float) ScreenController.BaseWidth;

            uiCamera.Y = height / 2;
            uiCamera.X = width / 2;

            ship = new Ship();

            timeText = new TextView(uiCamera, "0:00:00", Fonts.DEFAULT, 30, Color.White, width*0.01, height);
            timeText.HOrigin = HorizontalOrigin.LEFT;
            timeText.VOrigin = VerticalOrigin.BOTTOM;
            timeTextBox = new TextureView(uiCamera, Textures.SQUARE, new Color(0,0,0,150), 0, height, timeText.Width*1.2f, timeText.Height*1.2f);
            timeTextBox.HOrigin = HorizontalOrigin.LEFT;
            timeTextBox.VOrigin = VerticalOrigin.BOTTOM;
            AddChildren(timeTextBox, timeText);

            countdownText = new TextView(uiCamera, "3...", Fonts.DEFAULT, 40, Color.White, width / 2, height / 4);
            countdownText.Hidden = true;
            countdownTextBox = new TextureView(uiCamera, Textures.SQUARE, new Color(0, 0, 0, 150), countdownText.X, countdownText.Y, countdownText.Width * 1.2f, countdownText.Height * 1.2f);
            countdownTextBox.Hidden = true;
            AddChildren(countdownTextBox, countdownText);



            Restart();
        }


        private async void Restart() {
            ship.X = 0;
            ship.Y = 0;
            ship.AccelerationX = 0;
            ship.AccelerationY = 0;
            ship.VelocityX = 0;
            ship.VelocityY = 0;
            stopWatch.Reset();
            shipPaused = true;

            // Countdown
            await Task.Delay(startCountdown_TickRate);
            countdownText.Hidden = false;
            countdownTextBox.Hidden = false;
            countdownText.Text = "3...";
            await Task.Delay(startCountdown_TickRate);
            countdownText.Text = "2...";
            await Task.Delay(startCountdown_TickRate);
            countdownText.Text = "1...";
            await Task.Delay(startCountdown_TickRate);
            countdownText.Text = "Go!";
            stopWatch.Start();
            shipPaused = false;
            await Task.Delay(startCountdown_TickRate);
            countdownText.Hidden = true;
            countdownTextBox.Hidden = true;
        }
        
        

        protected override bool OnKeyInput(KeyEventArgs e) {
            if (e.Action == KeyAction.PRESSED) {
                // Quit track
                if (e.Key == Keys.Escape) {
                    RequestSceneSwitch(new TrackSelectionScene());
                    return true;
                }

                if( e.Key == Keys.R) {
                    Restart();
                    return true;
                }
            }

            if( e.Action == KeyAction.HELD ) {
                if (!shipPaused) {

                    if (e.Key == Keys.Left) {
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
                        return true;
                    }
                }
            }

            if( e.Action == KeyAction.RELEASED) {

                if( e.Key == Keys.Space) {
                    ship.AccelerationX = 0;
                    ship.AccelerationY = 0;
                    return true;
                }
            }



           
            return false;
        }

        private void UpdateTimeText() {
            string time = "";
            time += stopWatch.Elapsed.Minutes.ToString("0:");
            time += stopWatch.Elapsed.Seconds.ToString("00:");
            time += (stopWatch.Elapsed.Milliseconds/10).ToString("00");

            timeText.Text = time;  
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

            UpdateTimeText();
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