using Godot;
using System;

namespace SpaceStationBuilder
{
	/// <summary>
	/// The base type of the tile. Only used to differentiate between empty space and
	/// the floor (more like space station structure/scaffolding/hull). Other types of 
	/// objects like doors, walls, etc. will be implemented as InstalledObjects that are 
	/// sitting on the floor.
	/// </summary>
	public enum TileType { Empty, Floor };

	public class Tile
	{
		private TileType _type;
		private TileType _oldType;
		public TileType Type
		{
			get { return _type; }
			set {
				_oldType = _type;
				_type = value;
				if (_oldType != _type && cbTileTypeChanged != null)
				{
					cbTileTypeChanged(this);
				}
			}
		}

		/// <summary>
		/// The delegate for a tile changing its type.
		/// </summary>
		Action<Tile> cbTileTypeChanged;

		/// <summary>
		/// This represents any type of movable object, like resources, tools, crates, etc.
		/// </summary>
		LooseObject looseObject;

		/// <summary>
		/// This represents constructed, immovable objects, like doors, walls, furniture, etc.
		/// </summary>
		InstalledObject installedObject;

		private World world;
		private readonly int _x;
		private readonly int _y;

		public int X { get => _x; }
		public int Y { get => _y; }


		/// <summary>
		/// Constructor for a <see cref="Tile"/> object.
		/// </summary>
		/// <param name="world">An instance of the <see cref="World"/> this tile is part of.</param>
		/// <param name="x">The x coordinate of this tile.</param>
		/// <param name="y">The y coordinate of this tile.</param>
		public Tile(World world, int x, int y)
		{
			this.world = world;
			_x = x;
			_y = y;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public bool PlaceObject(InstalledObject objInstance)
		{
			if (objInstance == null)
			{
				// Uninstalling object
				installedObject = null;
				return true;
			}

			if (installedObject != null)
			{
				GD.PrintErr("Tile " + X + "," + Y + ": Unable to install object. This tile already has one.");
				return false;
			}

			installedObject = objInstance;
			return true;
		}

		/// <summary>
		/// Register a callback for changing the <see cref="TileType"/> of this tile./>
		/// </summary>
		public void RegisterTileTypeChangedCallback(Action<Tile> callback)
		{
			cbTileTypeChanged += callback;
		}

		/// <summary>
		/// Unregister a callback.
		/// </summary>
		public void UnregisterTileTypeChangedCallback(Action<Tile> callback)
		{
			cbTileTypeChanged -= callback;
		}
	}

}

