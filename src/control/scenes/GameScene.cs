﻿


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
        private BorderView border_Screen;
        private BorderView border_Rendering;

        private Stopwatch stopWatch = new Stopwatch();

        private int startCountdown_TickRate = 1000; // ms 

        private bool shipPaused = false;

        private bool collisionEnabled = true;

        private bool onlineTrack = false;

        private int blocksDrawn = 0;

        private bool crashed = true;

        private float targetZoom = 7f;


        private DebugInfoLine 
            infoLine_ShipPos,
            infoLine_ShipVel,
            infoLine_ShipRes,
            infoLine_ShipAcc,
            infoLine_CollisionEnabled,
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

            // Create background
            //Color wallColor = ;
            
            //wallColor.A = 255;
            //BackgroundColor = wallColor;

            ship = new Ship(gameCamera);
            AddChild(ship);

            timeText = new TextView(uiCamera, "0:00:00", Font.DEFAULT, 30, Color.White, width*0.01, height);
            timeText.HOrigin = HorizontalOrigin.LEFT;
            timeText.VOrigin = VerticalOrigin.BOTTOM;
            timeTextBox = new TextureView(uiCamera, Textures.SQUARE, new Color(0,0,0,150), 0, height, timeText.Width*1.2f, timeText.Height*1.2f);
            timeTextBox.HOrigin = HorizontalOrigin.LEFT;
            timeTextBox.VOrigin = VerticalOrigin.BOTTOM;
            AddChildren(timeTextBox);
            timeTextBox.AddChild(timeText);

            countdownText = new TextView(uiCamera, "3...", Font.DEFAULT, 40, Color.White, width / 2, height / 4);
            countdownText.Hidden = true;
            countdownTextBox = new TextureView(uiCamera, Textures.SQUARE, new Color(0, 0, 0, 150), countdownText.X, countdownText.Y, countdownText.Width * 1.2f, countdownText.Height * 1.2f);
            countdownTextBox.Hidden = true;
            AddChildren(countdownTextBox);
            countdownTextBox.AddChild(countdownText);

            border_Screen = new BorderView(gameCamera, color: Color.Red, borderWidth: 1, width: width/7, height: height/7 );
            border_Rendering = new BorderView(gameCamera, color: Color.Blue, borderWidth: 1, width: Settings.TRACK_RENDER_DISTANCE, height:  Settings.TRACK_RENDER_DISTANCE );
            //AddChildren(border_Screen, border_Rendering);

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

            infoLine_ShipVel = DebugOverlay.Info.AddInfoLine("Ship vel (V)", "?", (infoLine) => {
                infoLine.Info = string.Format("{0:N2}", Math.Abs(ship.VelocityX)+Math.Abs(ship.VelocityY) );
            });

            infoLine_ShipAcc = DebugOverlay.Info.AddInfoLine("Ship acc (A)", "?", (infoLine) => {
                infoLine.Info = string.Format("{0:N0}", Settings.SHIP_ACCELERATION);
            });

            infoLine_ShipRes = DebugOverlay.Info.AddInfoLine("Ship res (E)", "?", (infoLine) => {
                infoLine.Info = string.Format("{0:N2}", ship.Resistance);
            });

            infoLine_CollisionEnabled = DebugOverlay.Info.AddInfoLine("Coll. On (C)", "?", (infoLine) => {
                infoLine.Info = collisionEnabled.ToString();
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
            DebugOverlay.Info.RemoveInfoLine(infoLine_ShipAcc);
            DebugOverlay.Info.RemoveInfoLine(infoLine_ShipVel);
            DebugOverlay.Info.RemoveInfoLine(infoLine_ShipRes);
            DebugOverlay.Info.RemoveInfoLine(infoLine_CollisionEnabled);
            DebugOverlay.Info.RemoveInfoLine(infoLine_ChunksDrawn);
            DebugOverlay.Info.RemoveInfoLine(infoLine_CameraZoom);
        }


        private async void Restart() {
            ship.X = track.StartX;
            ship.Y = track.StartY;
            ship.RotationVelocity = 0;
            ship.Rotation = (float) track.StartRotation;
            
            ship.AccelerationX = 0;
            ship.AccelerationY = 0;
            ship.VelocityX = 0;
            ship.VelocityY = 0;
            stopWatch.Reset();
            shipPaused = true;
            crashed = false;
            ship.Hidden = false;

            // Smooth zoom motion on start (just because its pretty)
            gameCamera.Zoom = DEFAULT_ZOOM-0.5f;
            targetZoom = DEFAULT_ZOOM;

            // Reset checkpoints
            foreach( var checkpoint in track.Checkpoints) {
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

                // TODO: Remove this
                if( e.Key == Keys.C) {
                    collisionEnabled = !collisionEnabled;
                    return true;
                }

                if (e.Key == Keys.A) {
                    Settings.SHIP_ACCELERATION = (float) ((Settings.SHIP_ACCELERATION+30f) % 400f);
                    return true;
                }

                if (e.Key == Keys.E) {
                    ship.Resistance = (ship.Resistance+0.20f) % 4f;
                    return true;
                }
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

                if( e.Key == Keys.X) {
                    gameCamera.Zoom *= 0.95f;
                    return true;
                }
                if( e.Key == Keys.Z) {
                    gameCamera.Zoom *= 1.05f;
                    return true;
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

        private void UpdateTimeText() {
            string time = "";
            time += stopWatch.Elapsed.Minutes.ToString("0:");
            time += stopWatch.Elapsed.Seconds.ToString("00:");
            time += (stopWatch.Elapsed.Milliseconds/10).ToString("00");

            timeText.Text = time;  
        }

        protected override void OnUpdate(double deltaTime) {

            // Check ship collision
            if( !crashed) {
                var shipCollision = false;
                if (collisionEnabled) {
                    track.ForChunkInRange((int)ship.X - 20, (int)ship.X + 20, (int)ship.Y - 20, (int)ship.Y + 20, chunk => {
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



        private void UpdateCameraZoom() {
            targetZoom = 7 - ship.Velocity / 300;
            gameCamera.Zoom += (targetZoom - gameCamera.Zoom)*0.05f;
        }


        private async void ShipCrashed() {
            crashed = true;
            shipPaused = true;
            ship.Hidden = true;
            await Task.Run(() => Thread.Sleep(2000));
            Restart();
        }


        



        protected override void OnDraw(Renderer renderer) {
            gameCamera.X = ship.X;
            gameCamera.Y = ship.Y;
            border_Screen.X = ship.X;
            border_Screen.Y = ship.Y;
            border_Rendering.X = ship.X;
            border_Rendering.Y = ship.Y;

            blocksDrawn = renderer.DrawTrack(gameCamera, track);
        }


    }

}