using Godot;
using System;

namespace SpaceStationBuilder
{
	public class BuildButtonContainer : Node
	{
		// Called when the node enters the scene tree for the first time.
		public override void _Ready()
		{
			// This is just for testing. Button will probably be created somewhere else, and can be added
			// to the BuildButtonGroup there as well.
			Button testButton = GetNode<Button>("./VBoxContainer/DoorButton");
			testButton.AddToGroup("BuildButtons");

			// Get all nodes in the BuildButtonGroup and connect their pressed signal to the method below,
			// called with the button as an argument
			foreach (Button b in GetTree().GetNodesInGroup("BuildButtons"))
			{
				b.Connect("pressed", this, nameof(_on_Button_pressed), new Godot.Collections.Array() { b });
			}
		}

		private void _on_Button_pressed(Button button)
		{
			string buttonName = button.Name;
			if (buttonName.EndsWith("Button"))
			{
				buttonName = buttonName.Remove(buttonName.Length - "Button".Length);
			}

			// TODO: This should probably be done in some sort of state machine.
			WorldController.Instance.BuildModeIsFurniture = true;
			WorldController.Instance.BuildModeFurnitureType = buttonName;
		}
	}
}
