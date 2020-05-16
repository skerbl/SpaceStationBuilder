using Godot;
using System;

namespace SpaceStationBuilder
{
    public class CameraParallax : Node2D
    {
        private Camera2D mainCamera;
        private Vector2 previousPosition = new Vector2(Vector2.Zero);
        private Vector2 newPosition = new Vector2(Vector2.Zero);

        [Export(PropertyHint.Range, "0,1")]
        private float parallaxStrength = 0.9f;

        // Called when the node enters the scene tree for the first time.
        public override void _Ready()
        {
            mainCamera = GetNode<Camera2D>("../Camera2D");
            previousPosition = mainCamera.Position;
        }

        // Called every frame. 'delta' is the elapsed time since the previous frame.
        public override void _Process(float delta)
        {
            newPosition = mainCamera.Position;
            this.Position += (newPosition - previousPosition) * parallaxStrength;
            previousPosition = newPosition;
        }
    }
}
