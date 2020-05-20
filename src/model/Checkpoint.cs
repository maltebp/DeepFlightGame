using Microsoft.Xna.Framework;

namespace DeepFlight.track {

    public class Checkpoint : TextureView {

        private bool reached = false;
        public bool Reached { get => reached; set {
                Hidden = value;
                reached = value;
        }}

        // Index is the checkpoint number in the given Track
        public int Index { get; }
        

        public Checkpoint(int index, Color color, double x, double y) : base(null, Textures.PIXEL_CIRCLE_16) {
            X = x;
            Y = y;
            Index = index;
            Color = color;
            Width = Settings.CHECKPOINT_SIZE;
            Height = Settings.CHECKPOINT_SIZE;
            RotationVelocity = 0.4f;

            // Add a slightly scaled down collider
            // Used to detect collision with ship
            AddCollider(new CircleCollider(this, 1.1f));
        }

        protected override void OnUpdate(double deltaTime) {
            // The ship rotates
            UpdateMovement(deltaTime);
        }

    }
}
