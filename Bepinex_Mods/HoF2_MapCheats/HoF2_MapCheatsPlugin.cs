using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using System;
using System.Reflection;
using UnityEngine;

namespace HoF2_MapCheats
{
	// TODO Review this file and update to your own requirements.

	[BepInPlugin(MyGUID, PluginName, VersionString)]
	public class HoF2_MapCheatsPlugin : BaseUnityPlugin
	{
		// Mod specific details. MyGUID should be unique, and follow the reverse domain pattern
		// e.g.
		// com.mynameororg.pluginname
		// Version should be a valid version string.
		// e.g.
		// 1.0.0
		private const string MyGUID = "com.Wayden.HoF2_MapCheats";
		private const string PluginName = "HoF2_MapCheats";
		private const string VersionString = "1.0.0";

		public enum e_Cheats
		{
			FreeRoam,
			IgnoreFame,
			Gold,
			Fame,
			Life,
			Food,
			MaxHp,
			FlipMap,
			Amount,
		}

		public static ConfigEntry<bool>[] CheatsConfigs = new ConfigEntry<bool>[Enum.GetValues(typeof(e_Cheats)).Length];
		public static ConfigEntry<int> ConfigAmount;


		public static string[] CheatsKeys = new string[]{
			"FreeRoam",
			"Ignore Fame",
			"Apply to Gold",
			"Apply to Fame",
			"Apply to Health",
			"Apply to Food",
			"Apply to MaxHealth",
			"Flip Map Cards",
			"Amount",
		};

		public static bool Apply_to_gold = false;
		public static bool Apply_to_fame = false;
		public static bool Apply_to_life = false;
		public static bool Apply_to_food = false;
		public static bool Apply_to_maxhealth = false;
		public static bool Flip_Map_Card = false;

		public static readonly Action<bool>[] SetCheatFlags = new Action<bool>[]
		{
			newvalue => DebugMenu_Cheats.FreeRoam = newvalue,
			newvalue => DebugMenu_Cheats.IgnoreFame = newvalue,
			newvalue => HoF2_MapCheatsPlugin.Apply_to_gold = newvalue,
			newvalue => HoF2_MapCheatsPlugin.Apply_to_fame = newvalue,
			newvalue => HoF2_MapCheatsPlugin.Apply_to_life = newvalue,
			newvalue => HoF2_MapCheatsPlugin.Apply_to_food = newvalue,
			newvalue => HoF2_MapCheatsPlugin.Apply_to_maxhealth = newvalue,
			newvalue => HoF2_MapCheatsPlugin.Flip_Map_Card = newvalue,
		};

		private static readonly Harmony Harmony = new Harmony(MyGUID);
		public static ManualLogSource Log = new ManualLogSource(PluginName);

		/// <summary>
		/// Initialise the configuration settings and patch methods
		/// </summary>
		private void Awake()
		{
			SetUpBools();
			SetUpCheat();
			// Apply all of our patches
			Logger.LogInfo($"PluginName: {PluginName}, VersionString: {VersionString} is loading...");
			Harmony.PatchAll();
			Logger.LogInfo($"PluginName: {PluginName}, VersionString: {VersionString} is loaded.");

			Log = Logger;
		}

		/// <summary>
		/// Code executed every frame. 
		/// </summary>
		// TODO - Add your code here or remove this section if not required.
		private void Update()
		{
			ApplyCheats();
		}
		public static void ApplyCheats()
		{
			Player instance = Player.Instance;
			if (instance != null)
			{
				if (Flip_Map_Card)
				{
					if (Map.Instance != null && Map.Instance.MapLayout != null)
					{
						foreach (MapLayoutSlot mapLayoutSlot in Map.Instance.MapLayout.Slots)
						{
							if (mapLayoutSlot.Cards.Count > 0)
							{
								mapLayoutSlot.TopCard.Flipped = true;
								mapLayoutSlot.RepositionCards(null);
							}
						}
					}
					Flip_Map_Card = false;
					CheatsConfigs[(int)e_Cheats.FlipMap].Value = Flip_Map_Card;
				}
				if (Apply_to_fame)
				{
					instance.Fame.Set(instance.Fame.Get() + ConfigAmount.Value);
					Apply_to_fame = false;
					CheatsConfigs[(int)e_Cheats.Fame].Value = Apply_to_fame;
				}
				if (Apply_to_food)
				{
					instance.Food.Set(instance.Food.Get() + ConfigAmount.Value);
					Apply_to_food = false;
					CheatsConfigs[(int)e_Cheats.Food].Value = Apply_to_food;
				}
				if (Apply_to_maxhealth)
				{
					instance.MaxHealth.Set(instance.MaxHealth.Get() + (float)ConfigAmount.Value);
					Apply_to_maxhealth = false;
					CheatsConfigs[(int)e_Cheats.MaxHp].Value = Apply_to_maxhealth;
				}
				if (Apply_to_life)
				{
					instance.Health.Set(instance.Health.Get() + (float)ConfigAmount.Value);
					Apply_to_life = false;
					CheatsConfigs[(int)e_Cheats.Life].Value = Apply_to_life;
				}
				if (Apply_to_gold)
				{
					instance.Gold.Set(instance.Gold.Get() + ConfigAmount.Value);
					Apply_to_gold = false;
					CheatsConfigs[(int)e_Cheats.Gold].Value = Apply_to_gold;
				}
			}
		}

		public void SetUpBools()
		{
			foreach (e_Cheats value in Enum.GetValues(typeof(e_Cheats)))
			{
				//setup only les booleens
				if(value < e_Cheats.Amount)
				{
					CheatsConfigs[(int)value] = Config.Bind("Map Cheats", CheatsKeys[(int)value], false, "Activate " + CheatsKeys[(int)value]);
					CheatsConfigs[(int)value].SettingChanged += CheatManager;
					SetCheatFlags[(int)value](CheatsConfigs[(int)value].Value);
				}
			}
		}

		public void SetUpCheat()
		{
			ConfigAmount = Config.Bind("Resource Amount", CheatsKeys[(int)e_Cheats.Amount], 1, new ConfigDescription("Resources Amount to add", new AcceptableValueRange<int>(-1000, 1000)));
		}
		private void CheatManager(object sender, System.EventArgs a)
		{
			SettingChangedEventArgs settingChangedEventArgs = a as SettingChangedEventArgs;
			string Key = settingChangedEventArgs.ChangedSetting.Definition.Key;
			if (settingChangedEventArgs == null)
				return;
			int index = Array.IndexOf(CheatsKeys, Key);
			if (index != -1)
			{
				string msg;
				if (CheatsConfigs[index].Value)
				{
					SetCheatFlags[index](true);
					msg = string.Format("Cheat {0} has been Activated", CheatsKeys[index]);
				}
				else
				{
					SetCheatFlags[index](false);
					msg = string.Format("Cheat {0} has been Deactivated", CheatsKeys[index]);
				}
				HoF2_MapCheatsPlugin.Log.LogInfo(msg);
			}
		}
	}
}
