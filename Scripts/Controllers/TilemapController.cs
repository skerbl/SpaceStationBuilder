using Godot;
using System;
using System.Collections.Generic;

namespace SpaceStationBuilder
{
	public class TilemapController : Node2D
	{
		public TileType BuildModeType { protected get; set; }
		public bool BuildModeIsFurniture { protected get; set; } = false;
		public string BuildModeFurnitureType { protected get; set; }

		private TileMap tileMap;
		private Dictionary<string, int> tileIndexMap = new Dictionary<string, int>();

		private TileMap furnitureMap;
		private Dictionary<string, int> furnitureIndexMap = new Dictionary<string, int>();

		private World world { get { return WorldController.Instance.World; } }

		// Called when the node enters the scene tree for the first time.
		public override void _Ready()
		{
			//LoadFurnitureSprites();
			tileMap = GetNode<TileMap>("World");
			furnitureMap = GetNode<TileMap>("InstalledObjects");

			CreateTileDictionary();
			CreateFurnitureDictionary();

			world.cbFurnitureCreated += OnFurnitureCreated;
			world.cbTileChanged += OnTileChanged;

			for (int x = 0; x < world.Width; x++)
			{
				for (int y = 0; y < world.Height; y++)
				{
					world.GetTileAt(x, y).Type = TileType.Empty;
					tileMap.SetCell(x, y, tileIndexMap["Empty"]);
				}
			}

			// Maybe center the camera on the World?

			//World.RandomizeTiles();
			//World.SetAllTiles(TileType.Empty);
		}

		void LoadFurnitureSprites()
		{
			List<Resource> spriteResources = new List<Resource>();
			Directory dir = new Directory();
			dir.Open("Resources");
			string dirPath = dir.GetCurrentDir();
			dir.ListDirBegin();
			string fileName = dir.GetNext();

			while (fileName != "")
			{
				if (dir.CurrentIsDir())
				{

				}
				else if (fileName.ToLower().EndsWith(".png"))
				{
					Texture tex = GD.Load<Texture>(dir.GetCurrentDir() + "/" + fileName);
					tex.ResourceName = fileName.Remove(fileName.Length - ".png".Length);
					spriteResources.Add(tex);
				}
				else
				{

				}

				fileName = dir.GetNext();
			}
			dir.ListDirEnd();
		}

		/// <summary>
		/// The method that gets called as a callback whenever a tile's type changes.
		/// </summary>
		/// <param name="tileData"></param>
		void OnTileChanged(Tile tileData)
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
		void OnFurnitureCreated(Furniture obj)
		{
			if (furnitureIndexMap.ContainsKey(obj.Type))
			{
				furnitureMap.SetCell(obj.Tile.X, obj.Tile.Y, furnitureIndexMap[obj.Type]);

				// Autotiles currently don't update automatically, so this needs to be called manually
				furnitureMap.UpdateBitmaskArea(new Vector2(obj.Tile.X, obj.Tile.Y));
			}
			else
			{
				GD.Print("OnFurnitureCreated -- No visuals found in furniture map for type: " + obj.Type);
			}

			obj.cbOnChanged += OnFurnitureChanged;
		}

		/// <summary>
		/// The method that gets called whenever furniture gets changed (i.e. a door opening, or something 
		/// taking damage).
		/// For now this does exactly the same as OnFurnitureCreated. Will probably change (or disappear)
		/// once additional features/graphics are implemented.
		/// </summary>
		/// <param name="obj">The furniture that got changed</param>
		void OnFurnitureChanged(Furniture obj)
		{
			if (furnitureIndexMap.ContainsKey(obj.Type))
			{
				furnitureMap.SetCell(obj.Tile.X, obj.Tile.Y, furnitureIndexMap[obj.Type]);
				furnitureMap.UpdateBitmaskArea(new Vector2(obj.Tile.X, obj.Tile.Y));
			}
			else
			{
				GD.Print("OnFurnitureCreated -- No visuals found in furniture map for type: " + obj.Type);
			}
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
					t = world.GetTileAt(x, y);
					if (t != null)
					{
						if (BuildModeIsFurniture == true)
						{
							string furnitureType = BuildModeFurnitureType;
							if (world.IsFurniturePlacementValid(furnitureType, t) && t.pendingFurnitureJob == null)
							{
								Job job = new Job(t, (theJob) =>
								{
									world.PlaceFurniture(furnitureType, theJob.Tile);
									t.pendingFurnitureJob = null;
								});

								// FIXME: Manually setting flags like this is not ideal.
								t.pendingFurnitureJob = job;
								job.cbJobCancelled += (theJob) => { theJob.Tile.pendingFurnitureJob = null; };

								world.jobQueue.Enqueue(job);
								GD.Print("Job Queue size: " + world.jobQueue.Count);
							}

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
		private void CreateFurnitureDictionary()
		{
			Godot.Collections.Array installedObjectIDs = furnitureMap.TileSet.GetTilesIds();
			for (int i = 0; i < installedObjectIDs.Count; i++)
			{
				int tileID = (int)installedObjectIDs[i];
				furnitureIndexMap.Add(furnitureMap.TileSet.TileGetName(tileID), tileID);
			}
		}
	}
}
