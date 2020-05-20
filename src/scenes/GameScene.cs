


using DeepFlight.rendering;
using DeepFlight.src.gui.debugoverlay;
using DeepFlight.track;
using DeepFlight.utility.KeyboardController;
using DeepFlight.view.gui;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace DeepFlight.scenes {

    /// <summary>
    /// The GameScene controls the actual game. It's given a track 
    /// </summary>
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

        private int startCountdown_TickRate = 800; // ms

        private bool restarting = false;
        private bool shipPaused = false;
        private bool collisionEnabled = true;
        private bool onlineTrack = false;
        private int blocksDrawn = 0;
        private bool crashed = true;
        private float targetZoom = 7f;


        private DebugInfoLine 
            infoLine_ShipPos,
            infoLine_ShipVel,
            infoLine_ShipRot,
            infoLine_ChunksDrawn,
            infoLine_CameraZoom;

        

        public GameScene(Track track, bool onlineTrack) {
            this.track = track;
            this.onlineTrack = onlineTrack;
            gameCamera.Zoom = DEFAULT_ZOOM;
        }

        protected override void OnInitialize() {
            if (!track.BlockDataDeserialized)
                throw new InvalidOperationException("Track's block data has not been deserialized.");

            // Height of UI screen
            float height = (float) ScreenController.BaseHeight;
            float width = (float) ScreenController.BaseWidth;

            uiCamera.Y = height / 2;
            uiCamera.X = width / 2;

            ship = new Ship(gameCamera);
            AddChild(ship);

            // The stopwatch
            timeText = new TextView(uiCamera, "0:00:00", Font.DEFAULT, 30, Color.White, width*0.01, height);
            timeText.HOrigin = HorizontalOrigin.LEFT;
            timeText.VOrigin = VerticalOrigin.BOTTOM;
            timeTextBox = new TextureView(uiCamera, Textures.SQUARE, new Color(0,0,0,150), 0, height+1, timeText.Width*1.2f, timeText.Height*1.2f);
            timeTextBox.HOrigin = HorizontalOrigin.LEFT;
            timeTextBox.VOrigin = VerticalOrigin.BOTTOM;
            AddChildren(timeTextBox);
            timeTextBox.AddChild(timeText);

            // Countdown (before stopwatch starts)
            countdownText = new TextView(uiCamera, "3...", Font.DEFAULT, 40, Color.White, width / 2, height / 4);
            countdownText.Hidden = true;
            countdownTextBox = new TextureView(uiCamera, Textures.SQUARE, new Color(0, 0, 0, 150), countdownText.X+2, countdownText.Y-5, countdownText.Width * 1.2f, countdownText.Height * 1.1f);
            countdownTextBox.Hidden = true;
            AddChildren(countdownTextBox);
            countdownTextBox.AddChild(countdownText);

            // Add checkpoints drawables
            var checkpoints = track.Checkpoints;
            if (checkpoints == null)
                throw new ArgumentNullException("Track checkpoint list is null!");
            if (checkpoints.Length == 0)
                throw new ArgumentException("Track has no checkpoints!");

            foreach (var checkpoint in track.Checkpoints) {
                checkpoint.Reached = false;
                checkpoint.Camera = gameCamera;
                AddChild(checkpoint);
            }

            // Setup DebugOverlay
            infoLine_ShipPos = DebugOverlay.Info.AddInfoLine("Ship pos", "?", (infoLine) => {
                infoLine.Info = string.Format("({0:N1},{1:N1})", ship.X, ship.Y);
            });

            infoLine_ShipVel = DebugOverlay.Info.AddInfoLine("Ship vel", "?", (infoLine) => {
                infoLine.Info = string.Format("{0:N2}", Math.Abs(ship.VelocityX)+Math.Abs(ship.VelocityY) );
            });

            infoLine_ShipRot = DebugOverlay.Info.AddInfoLine("Ship rot", "?", (infoLine) => {
                infoLine.Info = $"{ship.Rotation:N2}";
            });

            infoLine_ChunksDrawn = DebugOverlay.Info.AddInfoLine("Chunks Drawn", "?", (infoLine) => {
                infoLine.Info = "" + blocksDrawn;
            });

            infoLine_CameraZoom = DebugOverlay.Info.AddInfoLine("Zoom", "?", (infoLine) => {
                infoLine.Info = string.Format("{0:N2}", gameCamera.Zoom);
            });


            Restart();
        }

        protected override void OnTerminate() {
            DebugOverlay.Info.RemoveInfoLine(infoLine_ShipPos);
            DebugOverlay.Info.RemoveInfoLine(infoLine_ShipVel);
            DebugOverlay.Info.RemoveInfoLine(infoLine_ChunksDrawn);
            DebugOverlay.Info.RemoveInfoLine(infoLine_CameraZoom);
            DebugOverlay.Info.RemoveInfoLine(infoLine_ShipRot);
        }


        private async void Restart() {
            if( !restarting) {
                restarting = true;

                ship.X = track.StartX;
                ship.Y = track.StartY;
                ship.RotationVelocity = 0;
                ship.Rotation = (float)track.StartRotation;

                ship.AccelerationX = 0;
                ship.AccelerationY = 0;
                ship.VelocityX = 0;
                ship.VelocityY = 0;
                stopWatch.Reset();
                shipPaused = true;
                crashed = false;
                ship.Hidden = false;

                // Smooth zoom motion on start (just because its pretty)
                gameCamera.Zoom = DEFAULT_ZOOM - 0.5f;
                targetZoom = DEFAULT_ZOOM;

                // Reset checkpoints
                foreach (var checkpoint in track.Checkpoints) {
                    checkpoint.Reached = false;
                }

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

                restarting = false;
            }            
        }


        private void TrackFinished() {
            RequestSceneSwitch(new TrackCompleteScene(track, stopWatch.Elapsed, onlineTrack));
        }
        
        

        protected override bool OnKeyInput(KeyEventArgs e) {
            if (e.Action == KeyAction.PRESSED) {
                // Quit track
                if (e.Key == Keys.Escape) {
                    RequestSceneSwitch(new MainMenuScene());
                    return true;
                }

                if( e.Key == Keys.R) {
                    Restart();
                    return true;
                }

                #if DEBUG
                if( e.Key == Keys.C) {
                    collisionEnabled = !collisionEnabled;
                    return true;
                }
                #endif
            }


            if( e.Action == KeyAction.HELD ) {
                if (!shipPaused) {

                    if (e.Key == Keys.Left) {
                        ship.RotationVelocity = -Settings.SHIP_ROTATION_VELOCITY;
                        return true;
                    }
                    if (e.Key == Keys.Right) {
                        ship.RotationVelocity = Settings.SHIP_ROTATION_VELOCITY;
                        return true;
                    }

                    if (e.Key == Keys.Space) {
                        //double rotation = ship.Rotation + Math.PI * 1.5;
                        ship.AccelerationY = (float)(Math.Sin(ship.Rotation) * Settings.SHIP_ACCELERATION);
                        ship.AccelerationX = (float)(Math.Cos(ship.Rotation) * Settings.SHIP_ACCELERATION);
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

                if( e.Key == Keys.Left || e.Key == Keys.Right) {
                    ship.RotationVelocity = 0;
                }
            }

            return false;
        }

        protected override void OnUpdate(double deltaTime) {

            // Check ship collision
            if( !crashed) {
                var shipCollision = false;
                if (collisionEnabled) {
                    track.ForChunksInRange((int)ship.X - 20, (int)ship.X + 20, (int)ship.Y - 20, (int)ship.Y + 20, chunk => {
                        foreach (var collisionBlock in chunk.CollisionBlocks) {
                            if (collisionBlock.CollidesWith(ship))
                                shipCollision = true;
                        }
                    });
                }

                // Check checkpoint collision
                var allCheckpointsReached = true;
                foreach (var checkpoint in track.Checkpoints) {
                    if (checkpoint.Reached) continue;

                    if (checkpoint.CollidesWith(ship))
                        checkpoint.Reached = true;
                    else
                        allCheckpointsReached = false;
                }

                if (allCheckpointsReached) {
                    TrackFinished();
                    return;
                }

                if (shipCollision) {
                    ShipCrashed();
                    return;
                }

                UpdateTimeText();
                UpdateCameraZoom();
            }
        }


        private void UpdateTimeText() {
            string time = "";
            time += stopWatch.Elapsed.Minutes.ToString("0:");
            time += stopWatch.Elapsed.Seconds.ToString("00:");
            time += (stopWatch.Elapsed.Milliseconds / 10).ToString("00");

            timeText.Text = time;
        }


        // The camera is zoomed further out when the ship
        // speeds up
        private void UpdateCameraZoom() {
            targetZoom = 7 - ship.Velocity / 100;
            gameCamera.Zoom += (targetZoom - gameCamera.Zoom)*0.05f;
        }


        private async void ShipCrashed() {
            crashed = true;
            shipPaused = true;
            ship.Hidden = true;
            await Task.Run(() => Thread.Sleep(1500));
            Restart();
        }


        protected override void OnDraw(Renderer renderer) {
            // Draw the ship and a 
            gameCamera.X = ship.X + ship.VelocityX * 0.03;
            gameCamera.Y = ship.Y + ship.VelocityY * 0.03;

            blocksDrawn = renderer.DrawTrack(gameCamera, track);
        }

    }
}