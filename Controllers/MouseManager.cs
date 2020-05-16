using Godot;
using System;

namespace SpaceStationBuilder
{
    public class MouseManager : Node2D
    {
        private TileMap _tileSelectionGrid;
        private TileMap _worldGrid;
        private Camera2D _mainCamera;

        private Vector2 _tilePosition = new Vector2(Vector2.Zero);
        private Vector2 _oldTilePosition = new Vector2(Vector2.Zero);
        private Vector2 _previousMousePosition = new Vector2(Vector2.Zero);
        private bool _moveCamera = false;

        // Called when the node enters the scene tree for the first time.
        public override void _Ready()
        {
            _worldGrid = GetNode<TileMap>("../../WorldController/World");
            _tileSelectionGrid = GetNode<TileMap>("../TileSelectionGrid");
            _mainCamera = GetNode<Camera2D>("../../Camera2D");
        }

        public override void _Process(float delta)
        {
            
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
                    _previousMousePosition = clickEvent.Position;
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
                _mainCamera.Position += (_previousMousePosition - moveEvent.Position);
                _previousMousePosition = moveEvent.Position;
            }

            #endregion

            if (moveEvent != null)
            {
                _tilePosition = _tileSelectionGrid.WorldToMap(moveEvent.Position + _mainCamera.Position);

                if (_tilePosition != _oldTilePosition && _tileSelectionGrid.GetCellv(_tilePosition) == -1)
                {
                    _tileSelectionGrid.SetCellv(_tilePosition, 0);
                    _tileSelectionGrid.SetCellv(_oldTilePosition, -1);
                    _oldTilePosition = _tilePosition;
                }
                
                
            }
        }
    }
}