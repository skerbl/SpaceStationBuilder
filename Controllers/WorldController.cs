using Godot;
using System;

namespace SpaceStationBuilder
{
	public class WorldController : Node2D
	{
		private World world;
		private TileMap tileMap;
		private float randomizeTileTimer = 2f;

		// Called when the node enters the scene tree for the first time.
		public override void _Ready()
		{
			tileMap = GetNode<TileMap>("World");
			world = new World();

			for (int x = 0; x < world.Width; x++)
			{
				for (int y = 0; y < world.Height; y++)
				{
					// In Unity, I'd create a new GameObject here, and set its correct position according to the tile index. 
					// Give it a name and a SpriteRenderer, and set the Sprite according to its type.
					// newTile.name = "Tile_" + x + "_" + y;
					// newTile.transform.position = new Vector3(tileData.X, tileData.Y, 0);

					// In Godot, however, the TileMap handles all of that.

					//Tile tileData = world.GetTileAt(x, y);
					//tileMap.SetCell(x, y, (int)tileData.Type);
					world.GetTileAt(x, y).RegisterTileTypeChangedCallback(OnTileTypeChanged);
				}
			}

			world.RandomizeTiles();
		}

		void OnTileTypeChanged(Tile tileData)
		{
			tileMap.SetCell(tileData.X, tileData.Y, (int)tileData.Type);
		}

		//  // Called every frame. 'delta' is the elapsed time since the previous frame.
		public override void _Process(float delta)
		{
			randomizeTileTimer -= delta;
			if (randomizeTileTimer < 0)
			{
				world.RandomizeTiles();
				randomizeTileTimer = 2f;
			}
		}
	}
}
