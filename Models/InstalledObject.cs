using System;
using System.Collections.Generic;
using Godot;

namespace SpaceStationBuilder
{
	/// <summary>
	/// This represents constructed, immovable objects, like doors, walls, furniture, etc.
	/// </summary>
	public class InstalledObject
	{
		public static Dictionary<string, InstalledObject> Prototypes { get; protected set; } = new Dictionary<string, InstalledObject>();

		/// <summary>
		/// This represents the BASE tile of the object. For multi-tile objects, this would be
		/// the top left corner.
		/// </summary>
		public Tile Tile { get; protected set; }

		/// <summary>
		/// The type of the object.
		/// </summary>
		public string ObjectType { get; protected set; }

		/// <summary>
		/// A multiplier that (inversely) modifies the effective movement speed across the tile. Can be 
		/// combined with other such modifiers.
		/// SPECIAL: If set to 0, the tile will be impassable.
		/// </summary>
		float movementCost = 1f;

		int width = 1;
		int height = 1;

		/// <summary>
		/// This callback will be called whenever an installed object changes some of its state (i.e. a door opening or closing).
		/// </summary>
		Action<InstalledObject> cbOnChanged;

		// TODO: Implement multi-tile objects
		// TODO: Implement object rotation

		/// <summary>
		/// Protected constructor so objects of this class can't be constructed from the outside.
		/// </summary>
		protected InstalledObject()
		{

		}

		/// <summary>
		/// This is used to create the object prototype. Use <see cref="InstalledObject.Placeinstance(InstalledObject, Tile)"/> to place an object onto a tile.
		/// </summary>
		/// <param name="objectType">Identifies the object's type.</param>
		/// <param name="movementCost">A multiplier that (inversely) modifies the effective movement speed across the tile. Can be combined with others. Set to 0 for an impassable tile.</param>
		/// <param name="width">The width of the object from the upper left.</param>
		/// <param name="height">The height of the object from the upper left.</param>
		/// <returns>A reference to the prototype. Returns null if a prototype of the same name already exists.</returns>
		public static InstalledObject CreatePrototype(string objectType, float movementCost = 1f, int width = 1, int height = 1)
		{
			InstalledObject obj = new InstalledObject();
			obj.ObjectType = objectType;
			obj.movementCost = movementCost;
			obj.width = width;
			obj.height = height;

			if (Prototypes.ContainsKey(objectType))
			{
				GD.Print("Unable to create prototype " + objectType + ".");
				return null;
			}
			else
			{
				Prototypes.Add(objectType, obj);
				return obj;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="prototype">The prototype of the installed object.</param>
		/// <param name="tile">The tile it will be installed to.</param>
		/// <returns>A reference to the object instance on success, null on failure.</returns>
		public static InstalledObject Placeinstance(InstalledObject prototype, Tile tile)
		{
			InstalledObject obj = new InstalledObject();
			obj.ObjectType = prototype.ObjectType;
			obj.movementCost = prototype.movementCost;
			obj.width = prototype.width;
			obj.height = prototype.height;

			obj.Tile = tile;

			// FIXME: This assumes an object size of 1x1. Don't forget multi-tile objects.
			if (tile.PlaceObject(obj) == false)
			{
				// Object could not be placed, probably because the tile was already full.
				// Do not return the new object.

				return null;
			}

			return obj;
		}
	}
}
