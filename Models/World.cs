using Godot;
using System;


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
						tiles[x, y].Type = Tile.TileType.Empty;
					}
					else
					{
						tiles[x, y].Type = Tile.TileType.Floor;
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
			if (x > _width || x < 0 || y > _height || y < 0)
			{
				GD.Print("Tile (" + x + "," + y + ") is out of range!");
				return null;
			}
			return tiles[x, y];
		}
	}
}
