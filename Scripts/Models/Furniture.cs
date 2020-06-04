using System;
using System.Collections.Generic;
using Godot;

namespace SpaceStationBuilder
{
	/// <summary>
	/// This represents constructed, immovable objects, like doors, walls, furniture, etc.
	/// </summary>
	public class Furniture
	{
		public static Dictionary<string, Furniture> Prototypes { get; protected set; } = new Dictionary<string, Furniture>();

		/// <summary>
		/// This represents the BASE tile of the object. For multi-tile objects, this would be
		/// the top left corner.
		/// </summary>
		public Tile Tile { get; protected set; }

		/// <summary>
		/// The type of the object.
		/// </summary>
		public string Type { get; protected set; }

		/// <summary>
		/// A multiplier that (inversely) modifies the effective movement speed across the tile. Can be 
		/// combined with other such modifiers.
		/// SPECIAL: If set to 0, the tile will be impassable.
		/// </summary>
		float movementCost = 1f;

		int width = 1;
		int height = 1;

		/// <summary>
		/// Specifies whether the object cares about its neighbours being of the same type. For example walls, pipes, or power lines 
		/// that link up to form enclosed rooms or transmit water, electricity, etc.
		/// </summary>
		public bool LinksToNeighbours { get; protected set; } = false;

		/// <summary>
		/// This callback will be called whenever an installed object changes some of its state (i.e. a door opening or closing).
		/// </summary>
		Action<Furniture> cbOnChanged;

		public Func<Tile, bool> funcPositionValid;

		// TODO: Implement multi-tile objects
		// TODO: Implement object rotation

		/// <summary>
		/// Protected constructor so objects of this class can't be constructed from the outside.
		/// </summary>
		protected Furniture()
		{

		}

		/// <summary>
		/// This is used to create the object prototype. Use <see cref="Furniture.Placeinstance(Furniture, Tile)"/> 
		/// to place an object onto a tile.
		/// </summary>
		/// <param name="type">Identifies the object's type.</param>
		/// <param name="movementCost">A multiplier that (inversely) modifies the effective movement speed across the tile. 
		/// Can be combined with others. Set to 0 for an impassable tile.</param>
		/// <param name="width">The width of the object from the upper left.</param>
		/// <param name="height">The height of the object from the upper left.</param>
		/// <returns>A reference to the prototype. Returns null if a prototype of the same name already exists.</returns>
		public static Furniture CreatePrototype(string type, float movementCost = 1f, int width = 1, int height = 1, bool linksToNeighbours = false)
		{
			Furniture obj = new Furniture();
			obj.Type = type;
			obj.movementCost = movementCost;
			obj.width = width;
			obj.height = height;
			obj.LinksToNeighbours = linksToNeighbours;

			obj.funcPositionValid = obj.IsPositionValid;

			if (Prototypes.ContainsKey(type))
			{
				GD.Print("Unable to create prototype " + type + ".");
				return null;
			}
			else
			{
				Prototypes.Add(type, obj);
				return obj;
			}
		}

		/// <summary>
		/// Places an instance of this furniture prototype onto a tile.
		/// </summary>
		/// <param name="prototype">The prototype of the installed object.</param>
		/// <param name="tile">The tile it will be installed to.</param>
		/// <returns>A reference to the object instance on success, null on failure.</returns>
		public static Furniture Placeinstance(Furniture prototype, Tile tile)
		{
			if (prototype.funcPositionValid(tile) == false)
			{
				GD.Print("PlaceInstance -- Position invalid.");
				return null;
			}

			Furniture obj = new Furniture();
			obj.Type = prototype.Type;
			obj.movementCost = prototype.movementCost;
			obj.width = prototype.width;
			obj.height = prototype.height;
			obj.LinksToNeighbours = prototype.LinksToNeighbours;

			obj.Tile = tile;

			// FIXME: This assumes an object size of 1x1. Don't forget multi-tile objects.
			if (tile.PlaceFurniture(obj) == false)
			{
				// Object could not be placed, probably because the tile was already full.
				// Do not return the new object.

				return null;
			}

			return obj;
		}

		/// <summary>
		/// Checks the general condition for furniture placement: Is there a floor underneath?
		/// </summary>
		/// <param name="t">The tile</param>
		/// <returns>True or false</returns>
		private bool IsPositionValid(Tile t)
		{
			if (t.Type != TileType.Floor)
			{
				return false;
			}

			if (t.Furniture != null)
			{
				return false;
			}

			return true;
		}

		/// <summary>
		/// Checks the condition for door placement: Are there neighbours on either side (NS or EW)?
		/// </summary>
		/// <param name="t">The tile</param>
		/// <returns>True or false</returns>
		private bool IsPositionValidDoor(Tile t)
		{
			if (IsPositionValid(t) == false)
			{
				return false;
			}

			return true;
		}

		public void RegisterOnChangedCallback(Action<Furniture> callback)
		{
			cbOnChanged += callback;
		}

		public void UnregisterOnChangedCallback(Action<Furniture> callback)
		{
			cbOnChanged -= callback;
		}
	}
}
