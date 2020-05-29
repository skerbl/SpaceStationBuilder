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

		Action<InstalledObject> cbInstalledObjectCreated;

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
				}
			}

			GD.Print("World created with " + width * height + " tiles.");

			InstalledObject wallPrototype = InstalledObject.CreatePrototype("Wall", 0, 1, 1);

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

		public void PlaceInstalledObject(string objectType, Tile tile)
		{
			// FIXME: This assumes an object size of 1x1. Don't forget multi-tile objects.
			if (InstalledObject.Prototypes.ContainsKey(objectType) == false)
			{
				GD.Print("InstalledObject.Prototypes does not contain a prototype for key " + objectType);
				return;
			}

			InstalledObject obj = InstalledObject.Placeinstance(InstalledObject.Prototypes[objectType], tile);

			if (obj != null)
			{
				cbInstalledObjectCreated?.Invoke(obj);
			}
		}

		public void RegisterInstalledObjectCreated(Action<InstalledObject> callback)
		{
			cbInstalledObjectCreated += callback;
		}

		public void UnregisterInstalledObjectCreated(Action<InstalledObject> callback)
		{
			cbInstalledObjectCreated -= callback;
		}
	}
}