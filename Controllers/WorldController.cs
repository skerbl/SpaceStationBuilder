using Godot;
using System;
using System.Collections.Generic;

namespace SpaceStationBuilder
{
	public class WorldController : Node2D
	{
		public static WorldController Instance { get; protected set; }

		public World World { get; protected set; }
		public TileType BuildModeType { protected get; set; }
		public bool BuildModeIsObject { protected get; set; } = false;
		public string BuildModeObjectType { protected get; set; }

		// TODO: Maybe separate these out into their own controllers/managers?
		private TileMap tileMap;
		private Dictionary<string, int> tileIndexMap = new Dictionary<string, int>();

		private TileMap installedObjectMap;
		private Dictionary<string, int> installedObjectIndexMap = new Dictionary<string, int>();


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
			installedObjectMap = GetNode<TileMap>("InstalledObjects");
			World = new World();
			World.RegisterInstalledObjectCreated(OnInstalledObjectCreated);

			for (int x = 0; x < World.Width; x++)
			{
				for (int y = 0; y < World.Height; y++)
				{
					// In Unity, I'd create a new GameObject here, and set its correct position according to the tile index. 
					// Give it a name and a SpriteRenderer, and set the Sprite according to its type.
					// Tile tileData = World.GetTileAt(x, y);
					// GameObject tile_gameobject = new GameObject();
					// tileGameObjectMap.Add(tileData, tile_gameobject)
					// tile_gameobject.name = "Tile_" + x + "_" + y;
					// tile_gameobject.transform.position = new Vector3(tileData.X, tileData.Y, 0);
					// tile_gameobject.AddComponent<Spriterenderer>();
					// tileData.RegisterTileTypeChangedCallback( OnTileTypeChanged )

					// In Godot, however, the TileMap handles all of that, so we only need to register the callback
					// It could also be set with pre-made tilemaps
					World.GetTileAt(x, y).RegisterTileTypeChangedCallback(OnTileTypeChanged);
				}
			}

			CreateTileDictionary();
			CreateInstalledObjectDictionary();
			World.RandomizeTiles();
		}

		/// <summary>
		/// The method that gets called as a callback whenever a tile's type changes.
		/// </summary>
		/// <param name="tileData"></param>
		void OnTileTypeChanged(Tile tileData)
		{
			// In Unity, this would also take a GameObject from a dictionary<Tile, GameObject> to update the SpriteRenderer's Sprite

			string tileTypeName = tileData.Type.ToString();
			if (tileIndexMap.ContainsKey(tileTypeName))
			{
				tileMap.SetCell(tileData.X, tileData.Y, tileIndexMap[tileTypeName]);
			}
			else
			{
				GD.Print("Error: No tile found in tileset for type " + tileTypeName);
			}
		}

		/// <summary>
		/// The method that gets called as a callback whenever a new installed object gets created.
		/// </summary>
		/// <param name="obj"></param>
		void OnInstalledObjectCreated(InstalledObject obj)
		{
			if (installedObjectIndexMap.ContainsKey(obj.ObjectType))
			{
				installedObjectMap.SetCell(obj.Tile.X, obj.Tile.Y, installedObjectIndexMap[obj.ObjectType]);

				// Autotiles currently don't update automatically, so this needs to be called manually
				installedObjectMap.UpdateBitmaskArea(new Vector2(obj.Tile.X, obj.Tile.Y));
			}
			else
			{
				GD.Print("Error: No tile found in InstalledObjects for type " + obj.ObjectType);
			}

			obj.RegisterOnChangedCallback(OnInstalledObjectChanged);
		}

		void OnInstalledObjectChanged(InstalledObject obj)
		{
			GD.Print("OnInstalledObjectChanged -- Not implemented yet.");
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

		/// <summary>
		/// This performs the selected build action on all tiles covered by the grid selection box.
		/// </summary>
		/// <param name="boxStart">The top left corner of the selection box.</param>
		/// <param name="boxEnd">The bottom right corner of the selection box.</param>
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
						if (BuildModeIsObject == true)
						{
							// Assign Object type
							World.PlaceInstalledObject(BuildModeObjectType, t);

							// TODO: Maybe a simple state machine could handle the various build and selection modes?
							// Both WorldController and MouseController/UI could observe the state and act accordingly
						}
						else
						{
							t.Type = BuildModeType;
						}
					}
				}
			}
		}

		/// <summary>
		/// This gets all the defined tile graphics from the world tilemap and maps their names to their indices.
		/// </summary>
		private void CreateTileDictionary()
		{
			Godot.Collections.Array tileIDs = tileMap.TileSet.GetTilesIds();
			for (int i = 0; i < tileIDs.Count; i++)
			{
				int tileID = (int)tileIDs[i];
				tileIndexMap.Add(tileMap.TileSet.TileGetName(tileID), tileID);
			}
		}

		/// <summary>
		/// This gets all the defined object graphics from the installed object tilemap and maps their names to their indices.
		/// </summary>
		private void CreateInstalledObjectDictionary()
		{
			Godot.Collections.Array installedObjectIDs = installedObjectMap.TileSet.GetTilesIds();
			for (int i = 0; i < installedObjectIDs.Count; i++)
			{
				int tileID = (int)installedObjectIDs[i];
				installedObjectIndexMap.Add(installedObjectMap.TileSet.TileGetName(tileID), tileID);
			}
		}
	}
}
