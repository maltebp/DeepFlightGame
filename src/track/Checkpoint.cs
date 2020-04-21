using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeepFlight.track {
    public class Checkpoint : TextureView {

        private bool reached = false;
        public bool Reached { get => reached; set {
                Hidden = value;
                reached = value;
        }}

        public int Index { get; }
        

        public Checkpoint(int index, Color color, double x, double y) : base(null, Textures.PIXEL_CIRCLE_16) {
            X = x;
            Y = y;
            Index = index;
            Color = color;
            Width = Settings.GAME_CHECKPOINT_SIZE;
            Height = Settings.GAME_CHECKPOINT_SIZE;
            RotationVelocity = 0.4f;

            // Add a slightly scaled down collider
            AddCollider(new CircleCollider(this, 1.1f));
        }

        protected override void OnUpdate(double deltaTime) {
            Mover.Move(this, deltaTime);
        }

    }
}
