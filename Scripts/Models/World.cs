using Godot;
using System;
using System.Collections.Generic;

namespace SpaceStationBuilder
{
	public class World
	{
		/// <summary>
		/// A two-dimensional array to hold the tile data.
		/// </summary>
		private Tile[,] tiles;

		private int _width;
		/// <summary>
		/// The tile width of the world
		/// </summary>
		public int Width { get => _width; }

		private int _height;
		/// <summary>
		/// The tile height of the world
		/// </summary>
		public int Height { get => _height; }

		/// <summary>
		/// A simple queue that holds requested jobs. This will almost
		/// certainly be moved into a separate class that manages multiple
		/// of these queues, each containing specific job types.
		/// </summary>
		public Queue<Job> jobQueue;

		public event Action<Tile> cbTileChanged;
		public event Action<Furniture> cbFurnitureCreated;

		/// <summary>
		/// Constructor for the <see cref="World"/> object.
		/// </summary>
		/// <param name="width">The width in tiles</param>
		/// <param name="height">The height in tiles</param>
		public World(int width = 100, int height = 100)
		{
			_width = width;
			_height = height;

			tiles = new Tile[_width, _height];

			for (int x = 0; x < _width; x++)
			{
				for (int y = 0; y < _height; y++)
				{
					tiles[x, y] = new Tile(this, x, y);
					//tiles[x, y].RegisterTileTypeChangedCallback(OnTileChanged);
					tiles[x, y].cbTileChanged += OnTileChanged;
				}
			}

			// TODO: Refactor this jobQueue out into an own dedicated class that handles multiple job queues.
			jobQueue = new Queue<Job>();

			GD.Print("World created with " + width * height + " tiles.");

			// TODO: Find a better place for creating the prototypes. Maybe in a separate ResourceLoader class?
			Furniture wallPrototype = Furniture.CreatePrototype("Wall", 0, 1, 1, true);
			Furniture doorPrototype = Furniture.CreatePrototype("Door", 0, 1, 1, true);
		}

		/// <summary>
		/// This sets all tiles of the world to one type.
		/// </summary>
		/// <param name="type">The type</param>
		public void SetAllTiles(TileType type)
		{
			for (int x = 0; x < _width; x++)
			{
				for (int y = 0; y < _height; y++)
				{
					
					tiles[x, y].Type = type;
				}
			}
		}

		/// <summary>
		/// This randomizes the tiles in the <see cref="World"/>. For testing purposes only.
		/// </summary>
		public void RandomizeTiles()
		{
			RandomNumberGenerator rng = new RandomNumberGenerator();
			DateTime time = DateTime.Now;
			long unixTime = ((DateTimeOffset)time).ToUnixTimeSeconds();
			rng.Seed = (ulong)unixTime;
			for (int x = 0; x < _width; x++)
			{
				for (int y = 0; y < _height; y++)
				{
					if (rng.RandiRange(0, 2) == 0)
					{
						tiles[x, y].Type = TileType.Empty;
					}
					else
					{
						tiles[x, y].Type = TileType.Floor;
					}
				}
			}
		}

		/// <summary>
		/// Gets the tile data at position x and y.
		/// </summary>
		/// <param name="x">The x coordinate of the tile.</param>
		/// <param name="y">The y coordinate of the tile.</param>
		/// <returns>The requested <see cref="Tile"/>.</returns>
		public Tile GetTileAt(int x, int y)
		{
			if (x > Width - 1 || x < 0 || y > Height - 1 || y < 0)
			{
				GD.Print("Tile (" + x + "," + y + ") is out of range!");
				return null;
			}
			return tiles[x, y];
		}

		public void PlaceFurniture(string furnitureType, Tile tile)
		{
			// FIXME: This assumes furniture size of 1x1. Don't forget multi-tile objects.
			if (Furniture.Prototypes.ContainsKey(furnitureType) == false)
			{
				GD.Print("Furniture.Prototypes does not contain a prototype for key " + furnitureType);
				return;
			}

			Furniture obj = Furniture.Placeinstance(Furniture.Prototypes[furnitureType], tile);

			if (obj != null)
			{
				cbFurnitureCreated?.Invoke(obj);
			}
		}

		public bool IsFurniturePlacementValid(string furnitureType, Tile t)
		{
			return Furniture.Prototypes[furnitureType].funcValidatePosition(t);
		}

		void OnTileChanged(Tile t)
		{
			cbTileChanged?.Invoke(t);
		}
	}
}
