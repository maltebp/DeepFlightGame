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
        
        private static readonly float ROTATION_SPEED = 0.1f;

        public Checkpoint(int index, Color color, double x, double y) : base(null, Textures.PIXEL_CIRCLE_9) {
            X = x;
            Y = y;
            Index = index;
            Col = color;
            Width = Settings.GAME_CHECKPOINT_SIZE;
            Height = Settings.GAME_CHECKPOINT_SIZE;

            // Add a slightly scaled down collider
            AddCollider(new CircleCollider(this, 0.9f));
        }

        protected override void OnUpdate(double deltaTime) {
            Rotation += (float) (deltaTime * ROTATION_SPEED);
        }

    }
}
