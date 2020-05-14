using Godot;
using System;

namespace SpaceStationBuilder
{
	public class Tile
	{
		public enum TileType { Empty, Floor };

		private TileType _type;
		public TileType Type
		{
			get { return _type; }
			set {
				_type = value;
				cbTileTypeChanged?.Invoke(this);
			}
		}

		Action<Tile> cbTileTypeChanged;

		LooseObject looseObject;
		InstalledObject installedObject;

		private World world;
		private int _x;
		private int _y;

		public int X { get => _x; }
		public int Y { get => _y; }



		public Tile(World world, int x, int y)
		{
			this.world = world;
			this._x = x;
			this._y = y;
		}

		public void RegisterTileTypeChangedCallback(Action<Tile> callback)
		{
			cbTileTypeChanged += callback;
		}

		public void UnregisterTileTypeChangedCallback(Action<Tile> callback)
		{
			cbTileTypeChanged -= callback;
		}
	}

}

