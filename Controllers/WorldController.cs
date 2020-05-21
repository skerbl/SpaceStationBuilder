using Godot;
using System;

namespace SpaceStationBuilder
{
	public class WorldController : Node2D
	{
		public static WorldController Instance { get; protected set; }

		public World World { get; protected set; }
		public Tile.TileType BuildModeType { protected get; set; }

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

		public void GridBoxSelect(Vector2 boxStart, Vector2 boxEnd)
		{
			int start_x = (int)boxStart.x;
			int end_x = (int)boxEnd.x;
			if (end_x < start_x)
			{
				int temp = end_x;
				end_x = start_x;
				start_x = temp;
			}

			int start_y = (int)boxStart.y;
			int end_y = (int)boxEnd.y;
			if (end_y < start_y)
			{
				int temp = end_y;
				end_y = start_y;
				start_y = temp;
			}

			Tile t;
			for (int x = start_x; x <= end_x; x++)
			{
				for (int y = start_y; y <= end_y; y++)
				{
					t = World.GetTileAt(x, y);
					if (t != null)
					{
						t.Type = BuildModeType;
					}
				}
			}
		}
	}
}
