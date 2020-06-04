using Godot;
using System;
using System.Collections.Generic;

namespace SpaceStationBuilder
{
	public class WorldSFX : AudioStreamPlayer
	{
		WorldController worldController;
		Dictionary<string, AudioStreamSample> worldSFX = new Dictionary<string, AudioStreamSample>();
		float soundCooldown = 0f;

		public override void _Ready()
		{
			WorldController.Instance.World.RegisterTileChanged(OnTileTypeChanged);
			WorldController.Instance.World.RegisterFurnitureCreated(OnFurnitureCreated);
			LoadStockSounds();
		}

		public override void _Process(float delta)
		{
			if (soundCooldown > 0)
				soundCooldown -= delta;
		}

		public void PlaySound(string name)
		{
			Stream = worldSFX[name];
			Play(0);
			soundCooldown = 0.1f;
		}

		private void LoadStockSounds()
		{
			worldSFX.Add("Floor_OnCreated", GD.Load<AudioStreamSample>("res://Assets/sounds/Floor_OnCreated.wav"));
			worldSFX.Add("Wall_OnCreated", GD.Load<AudioStreamSample>("res://Assets/sounds/Wall_OnCreated.wav"));
		}

		void OnTileTypeChanged(Tile tileData)
		{
			if (soundCooldown > 0)
				return;

			if (worldSFX.ContainsKey(tileData.Type.ToString() + "_OnCreated"))
			{
				PlaySound(tileData.Type.ToString() + "_OnCreated");
			}
		}

		void OnFurnitureCreated(Furniture obj)
		{
			if (soundCooldown > 0)
				return;

			if (worldSFX.ContainsKey(obj.Type + "_OnCreated"))
			{
				PlaySound(obj.Type + "_OnCreated");
			}
		}
	}
}
