using Godot;
using System;

namespace SpaceStationBuilder
{
    public class MouseManager : Node2D
    {
        TileMap tileSelectionGrid;
        Camera2D mainCamera;
        Vector2 lastFramePosition;
        Vector2 currentFramePosition;

        // Called when the node enters the scene tree for the first time.
        public override void _Ready()
        {
            tileSelectionGrid = GetNode<TileMap>("../TileSelectionGrid");
            mainCamera = GetNode<Camera2D>("../../Camera2D");
        }

        // Called every frame. 'delta' is the elapsed time since the previous frame.
        public override void _Process(float delta)
        {
            currentFramePosition = GetGlobalMousePosition();

            if (Input.IsActionPressed("move_map"))
            {
                Vector2 difference = lastFramePosition - currentFramePosition;
                mainCamera.GlobalPosition += difference;
            }

            lastFramePosition = GetGlobalMousePosition();
        }
    }
}