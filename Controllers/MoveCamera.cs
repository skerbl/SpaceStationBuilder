using Godot;
using System;

namespace SpaceStationBuilder
{
	public class MoveCamera : Camera2D
	{
		Vector2 fixedTogglePoint;

		// Called when the node enters the scene tree for the first time.
		public override void _Ready()
		{
			fixedTogglePoint = new Vector2(Vector2.Zero);
		}

		// Called every frame. 'delta' is the elapsed time since the previous frame.
		public override void _Process(float delta)
		{
			if (Input.IsActionJustPressed("move_map"))
			{
				var current = GetViewport().GetMousePosition();
				fixedTogglePoint = current;
			}
			if (Input.IsActionPressed("move_map"))
			{
				MoveMap();
			}
		}

		private void MoveMap()
		{
			var current = GetViewport().GetMousePosition();
			GlobalPosition += (current - fixedTogglePoint) / 20;
			
		}
	}
}
