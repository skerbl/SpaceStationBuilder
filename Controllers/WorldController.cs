using Godot;
using System;

namespace SpaceStationBuilder
{
	public class WorldController : Node2D
	{
		public static WorldController Instance { get; protected set; }

		public World World { get; protected set; }

		private TileMap tileMap;

		

		// Called when the node enters the scene tree for the first time.
		public override void _Ready()
		{
			if (Instance != null)
			{
				GD.Print("There should only be one WorldController instance.");
			}
			else
			{
				Instance = this;
			}

			tileMap = GetNode<TileMap>("World");
			World = new World();

			for (int x = 0; x < World.Width; x++)
			{
				for (int y = 0; y < World.Height; y++)
				{
					// In Unity, I'd create a new GameObject here, and set its correct position according to the tile index. 
					// Give it a name and a SpriteRenderer, and set the Sprite according to its type.
					// Tile tileData = World.GetTileAt(x, y);
					// GameObject tile_gameobject = new GameObject();
					// tile_gameobject.name = "Tile_" + x + "_" + y;
					// tile_gameobject.transform.position = new Vector3(tileData.X, tileData.Y, 0);
					// tile_gameobject.AddComponent<Spriterenderer>();
					// tileData.RegisterTileTypeChangedCallback( (tile) => { OnTileTypeChanged(tile, tile_gameobject) })

					// In Godot, however, the TileMap handles all of that.
					// It could also be set with pre-made tilemaps

					World.GetTileAt(x, y).RegisterTileTypeChangedCallback(OnTileTypeChanged);
				}
			}

			World.RandomizeTiles();
		}
		
		// In Unity, this would also take a GameObject to update the SpriteRenderer's Sprite
		void OnTileTypeChanged(Tile tileData)
		{
			if (tileData.Type == Tile.TileType.Empty)
			{
				tileMap.SetCell(tileData.X, tileData.Y, -1);
			}
			else
			{
				tileMap.SetCell(tileData.X, tileData.Y, (int)tileData.Type);
			}
		}

		//  // Called every frame. 'delta' is the elapsed time since the previous frame.
		//public override void _Process(float delta)
		//{
		//	
		//}

		public bool IsTileWithinWorld(int x, int y)
		{
			if (x < 0 || x >= World.Width || y < 0 || y >= World.Height)
				return false;
			else
				return true;
		}
	}
}
