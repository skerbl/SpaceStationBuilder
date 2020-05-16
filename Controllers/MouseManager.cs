using Godot;
using System;

namespace SpaceStationBuilder
{
    public class MouseManager : Node2D
    {
        private TileMap tileSelectionGrid;
        private Camera2D mainCamera;
        private Vector2 _previousPosition = new Vector2(Vector2.Zero);
        private bool _moveCamera = false;

        // Called when the node enters the scene tree for the first time.
        public override void _Ready()
        {
            tileSelectionGrid = GetNode<TileMap>("../TileSelectionGrid");
            mainCamera = GetNode<Camera2D>("../../Camera2D");
        }

        // This gets called whenever an input event occurs that does not belong to a Control (i.e. GUI) element.
        public override void _UnhandledInput(InputEvent @event)
        {
            #region Screen dragging

            InputEventMouseButton clickEvent = @event as InputEventMouseButton;
            InputEventMouseMotion moveEvent = @event as InputEventMouseMotion;
            if (clickEvent != null && clickEvent.ButtonIndex == (int)ButtonList.Right)
            {
                GetTree().SetInputAsHandled();
                if (@event.IsPressed())
                {
                    _previousPosition = clickEvent.Position;
                    _moveCamera = true;
                }
                else
                {
                    _moveCamera = false;
                }
            }
            else if (moveEvent != null && _moveCamera)
            {
                GetTree().SetInputAsHandled();
                mainCamera.Position += (_previousPosition - moveEvent.Position);
                _previousPosition = moveEvent.Position;
            }

            #endregion

        }
    }
}