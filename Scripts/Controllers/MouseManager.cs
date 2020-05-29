using Godot;
using System;

namespace SpaceStationBuilder
{
	public class MouseManager : Node2D
	{
		private TileMap tileSelectionGrid;
		private TileMap worldGrid;
		private Camera2D mainCamera;
		private TileType buildModeTile;

		[Export]
		private float minimumZoom = 0.5f;
		[Export]
		private float maximumZoom = 2f;
		[Export]
		private float zoomStep = 0.05f;

		private Vector2 tilePos = new Vector2(Vector2.Zero);
		private Vector2 oldTilePos = new Vector2(Vector2.Zero);
		private Vector2 dragStartPos = new Vector2(Vector2.Zero);
		private Vector2 dragEndPos = new Vector2(Vector2.Zero);
		private Vector2 previousMousePosition = new Vector2(Vector2.Zero);
		private Vector2 zoomMin;
		private Vector2 zoomMax;
		private Vector2 zoomFactor;
		private bool moveCamera = false;
		private bool leftMouseDrag = false;

		// Called when the node enters the scene tree for the first time.
		public override void _Ready()
		{
			worldGrid = GetNode<TileMap>("../WorldController/World");
			tileSelectionGrid = GetNode<TileMap>("./TileSelectionGrid");
			mainCamera = GetNode<Camera2D>("../Camera2D");

			zoomMin = new Vector2(minimumZoom, minimumZoom);
			zoomMax = new Vector2(maximumZoom, maximumZoom);
			zoomFactor = new Vector2(zoomStep, zoomStep);
		}

		public override void _Process(float delta)
		{
			if (leftMouseDrag)
			{
				ClearGridSelection();
				tilePos = tileSelectionGrid.WorldToMap(GetGlobalMousePosition().Round());
				UpdateDragging();
			}
		}

		// This gets called whenever an input event occurs that does not belong to a Control (i.e. GUI) element.
		public override void _UnhandledInput(InputEvent @event)
		{
			InputEventMouseButton clickEvent = @event as InputEventMouseButton;
			InputEventMouseMotion moveEvent = @event as InputEventMouseMotion;

			#region Left click

			if (clickEvent != null && clickEvent.ButtonIndex == (int)ButtonList.Left && clickEvent.IsPressed())
			{
				GetTree().SetInputAsHandled();
				leftMouseDrag = true;
				dragStartPos = tileSelectionGrid.WorldToMap(GetGlobalMousePosition().Round());
			}

			if (clickEvent != null && clickEvent.ButtonIndex == (int)ButtonList.Left && !clickEvent.IsPressed())
			{
				GetTree().SetInputAsHandled();
				dragEndPos = tileSelectionGrid.WorldToMap(GetGlobalMousePosition().Round());
				WorldController.Instance.GridBoxSelect(dragStartPos, dragEndPos);
				ClearGridSelection();
				leftMouseDrag = false;
			}

			#endregion

			#region Screen dragging

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
				mainCamera.Position += (previousMousePosition - moveEvent.Position) * mainCamera.Zoom;
				previousMousePosition = moveEvent.Position;
			}

			#endregion

			#region Zoom camera

			if (clickEvent != null && clickEvent.Pressed && clickEvent.ButtonIndex == (int)ButtonList.WheelUp && mainCamera.Zoom > zoomMin)
			{
				GetTree().SetInputAsHandled();
				Vector2 cameraZoom = (mainCamera.Zoom - zoomFactor);
				mainCamera.Zoom = cameraZoom;
			}

			if (clickEvent != null && clickEvent.Pressed && clickEvent.ButtonIndex == (int)ButtonList.WheelDown && mainCamera.Zoom < zoomMax)
			{
				GetTree().SetInputAsHandled();
				Vector2 cameraZoom = (mainCamera.Zoom + zoomFactor);
				mainCamera.Zoom = cameraZoom;
			}

			#endregion
			//#region Grid selection cursor

			//if (moveEvent != null)
			//{
			//    tilePos = tileSelectionGrid.WorldToMap(moveEvent.Position + mainCamera.Position);

			//    if (tilePos != oldTilePos && tileSelectionGrid.GetCellv(tilePos) == -1)
			//    {
			//        //if (tilePos.x < 0 || tilePos.x >= world.Width || tilePos.y < 0 || tilePos.y >= world.Height)
			//        if (!WorldController.Instance.IsTileWithinWorld(tilePos))
			//        {
			//            tileSelectionGrid.SetCellv(tilePos, -1);
			//            tileSelectionGrid.SetCellv(oldTilePos, -1);
			//            oldTilePos = tilePos;
			//        }
			//        else
			//        {
			//            tileSelectionGrid.SetCellv(tilePos, 0);
			//            tileSelectionGrid.SetCellv(oldTilePos, -1);
			//            oldTilePos = tilePos;
			//        }
			//    }
			//}

			//#endregion
		}

		private void UpdateDragging()
		{
			int start_x = (int)dragStartPos.x;
			int end_x = (int)tilePos.x;
			if (end_x < start_x)
			{
				int temp = end_x;
				end_x = start_x;
				start_x = temp;
			}

			int start_y = (int)dragStartPos.y;
			int end_y = (int)tilePos.y;
			if (end_y < start_y)
			{
				int temp = end_y;
				end_y = start_y;
				start_y = temp;
			}

			for (int x = start_x; x <= end_x; x++)
			{
				for (int y = start_y; y <= end_y; y++)
				{
					tileSelectionGrid.SetCell(x, y, 0);
				}
			}
		}

		private void ClearGridSelection()
		{
			int start_x = (int)dragStartPos.x;
			int end_x = (int)tilePos.x;
			if (end_x < start_x)
			{
				int temp = end_x;
				end_x = start_x;
				start_x = temp;
			}

			int start_y = (int)dragStartPos.y;
			int end_y = (int)tilePos.y;
			if (end_y < start_y)
			{
				int temp = end_y;
				end_y = start_y;
				start_y = temp;
			}

			for (int x = start_x; x <= end_x; x++)
			{
				for (int y = start_y; y <= end_y; y++)
				{
					tileSelectionGrid.SetCell(x, y, -1);
				}
			}
		}

		private void _on_BuildFloorButton_pressed()
		{
			WorldController.Instance.BuildModeIsObject = false;
			WorldController.Instance.BuildModeType = TileType.Floor;
		}


		private void _on_DeconstructButton_pressed()
		{
			WorldController.Instance.BuildModeIsObject = false;
			WorldController.Instance.BuildModeType = TileType.Empty;
		}

		/*
		private void SetModeBuildInstalledObject(string objectType)
		{
			WorldController.Instance.BuildModeIsObject = true;
			WorldController.Instance.BuildModeObjectType = objectType;
		}
		*/
	}
}
