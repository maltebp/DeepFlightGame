

/// <summary>
/// The Ship entity, consisting of a movable Ship Texture and a
/// Triangle collider
/// </summary>
class Ship : TextureView {

    public Ship(Camera camera) : base(camera, Textures.SHIP, Settings.SHIP_COLOR, 0,0, 5, 3.2f) {
        Resistance = Settings.SHIP_RESISTANCE;
        AddCollider(new TriangleCollider(this));
    }

    // Update the ship's movement
    protected override void OnUpdate(double deltaTime) {
        UpdateMovement(deltaTime);
    }

}