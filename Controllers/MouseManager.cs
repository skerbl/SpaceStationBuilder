using Godot;
using System;

namespace SpaceStationBuilder
{
    public class MouseManager : Node2D
    {
        private TileMap tileSelectionGrid;
        private TileMap worldGrid;
        private Camera2D mainCamera;

        private Vector2 tilePos = new Vector2(Vector2.Zero);
        private Vector2 oldTilePos = new Vector2(Vector2.Zero);
        private Vector2 previousMousePosition = new Vector2(Vector2.Zero);
        private bool moveCamera = false;

        // Called when the node enters the scene tree for the first time.
        public override void _Ready()
        {
            worldGrid = GetNode<TileMap>("../../WorldController/World");
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
                    previousMousePosition = clickEvent.Position;
                    moveCamera = true;
                }
                else
                {
                    moveCamera = false;
                }
            }
            else if (moveEvent != null && moveCamera)
            {
                GetTree().SetInputAsHandled();
                mainCamera.Position += (previousMousePosition - moveEvent.Position);
                previousMousePosition = moveEvent.Position;
            }

            #endregion

            #region Grid selection cursor

            if (moveEvent != null)
            {
                tilePos = tileSelectionGrid.WorldToMap(moveEvent.Position + mainCamera.Position);

                if (tilePos != oldTilePos && tileSelectionGrid.GetCellv(tilePos) == -1)
                {
                    //if (tilePos.x < 0 || tilePos.x >= world.Width || tilePos.y < 0 || tilePos.y >= world.Height)
                    if (!WorldController.Instance.IsTileWithinWorld((int)tilePos.x, (int)tilePos.y))
                    {
                        tileSelectionGrid.SetCellv(tilePos, -1);
                        tileSelectionGrid.SetCellv(oldTilePos, -1);
                        oldTilePos = tilePos;
                        GD.Print("Out of this world!");
                    }
                    else
                    {
                        tileSelectionGrid.SetCellv(tilePos, 0);
                        tileSelectionGrid.SetCellv(oldTilePos, -1);
                        oldTilePos = tilePos;
                        GD.Print("This is fine.");
                    }
                }
            }

            #endregion
        }
    }
}