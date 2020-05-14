using Godot;
using System;


namespace SpaceStationBuilder
{
    public class World
    {
        private Tile[,] tiles;

        private int _width;
        public int Width { get => _width; }

        private int _height;
        public int Height { get => _height; }

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
