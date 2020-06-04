using Godot;
using System;
using System.Collections.Generic;

namespace SpaceStationBuilder
{
	public class WorldController : Node2D
	{
		public static WorldController Instance { get; protected set; }

		public World World { get; protected set; }

		private WorldSFX worldAudioPlayer;

		public override void _EnterTree()
		{
			//base._EnterTree();
			if (Instance != null)
			{
				GD.Print("There should only be one WorldController instance.");
			}
			else
			{
				Instance = this;
			}

			World = new World();
		}

		// Called when the node enters the scene tree for the first time.
		public override void _Ready()
		{
			worldAudioPlayer = GetNode<WorldSFX>("WorldAudioPlayer");
		}

		/// <summary>
		/// This checks whether a given tile index is within the world boundaries.
		/// </summary>
		/// <param name="tilePosition">The x and y coordinates of a tile as represented in the tilemap.</param>
		/// <returns>True or false.</returns>
		public bool IsTileWithinWorld(Vector2 tilePosition)
		{
			int x = (int)tilePosition.x;
			int y = (int)tilePosition.y;
			if (x < 0 || x >= World.Width || y < 0 || y >= World.Height)
				return false;
			else
				return true;
		}

		/// <summary>
		/// Gets the tile at the given x and y coordinates.
		/// </summary>
		/// <param name="tilePosition">The x and y coordinates of a tile as represented in the tilemap.</param>
		/// <returns>The tile. Will return null if trying to access an index outside of world boundaries.</returns>
		public Tile GetTileAtPosition(Vector2 tilePosition)
		{
			int x = (int)tilePosition.x;
			int y = (int)tilePosition.y;
			return World.GetTileAt(x, y);
		}
	}
}
