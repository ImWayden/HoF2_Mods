using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using System;


namespace Hof2_MinigamesCheats
{
    // TODO Review this file and update to your own requirements.

    [BepInPlugin(MyGUID, PluginName, VersionString)]
    public class Hof2_MinigamesCheatsPlugin : BaseUnityPlugin
    {
        // Mod specific details. MyGUID should be unique, and follow the reverse domain pattern
        // e.g.
        // com.mynameororg.pluginname
        // Version should be a valid version string.
        // e.g.
        // 1.0.0
        private const string MyGUID = "com.Wayden.Hof2_MinigamesCheats";
        private const string PluginName = "HoF2 - Minigames Cheats";
        private const string VersionString = "1.0.0";

		public enum BoolCheats
		{
			DiceCheat,
			PendulumCheat,
			ChanceCardCheat,
			WheelOFFortuneCheat,
		}

		private ConfigEntry<bool>[] CheatsConfigs = new ConfigEntry<bool>[Enum.GetValues(typeof(BoolCheats)).Length];
		public string[] CheatsKeys = new string[]{
			"DiceCheat",
			"PendulumCheat",
			"ChanceCardCheat",
			"WheelOFFortuneCheat",
		};
		public static bool PendulumCheat = false;
		public static bool ChanceCardCheat = false;
		public static bool WheelOFFortuneCheat = false;
		private static readonly Action<bool>[] SetCheatFlags = new Action<bool>[]
		{
			newvalue => DebugMenu_Cheats.DiceCheat = newvalue,
			newvalue => Hof2_MinigamesCheatsPlugin.PendulumCheat = newvalue,
			newvalue => Hof2_MinigamesCheatsPlugin.ChanceCardCheat = newvalue,
			newvalue => Hof2_MinigamesCheatsPlugin.WheelOFFortuneCheat = newvalue,
		};

		public static ConfigEntry<KeyboardShortcut> KeyboardShortcutExample;

        private static readonly Harmony Harmony = new Harmony(MyGUID);
        public static ManualLogSource Log = new ManualLogSource(PluginName);

        /// <summary>
        /// Initialise the configuration settings and patch methods
        /// </summary>
        private void Awake()
        {
			//setting up the config cheats entry
			foreach (BoolCheats value in Enum.GetValues(typeof(BoolCheats)))
			{
				CheatsConfigs[(int)value] = Config.Bind("CheatMinigames", CheatsKeys[(int)value], false, "Activate " + CheatsKeys[(int)value]);
				CheatsConfigs[(int)value].SettingChanged += CheatManager;
				SetCheatFlags[(int)value](CheatsConfigs[(int)value].Value);
			}
            Logger.LogInfo($"PluginName: {PluginName}, VersionString: {VersionString} is loading...");
            Harmony.PatchAll();
            Logger.LogInfo($"PluginName: {PluginName}, VersionString: {VersionString} is loaded.");
            Log = Logger;
        }

        /// <summary>
        /// Code executed every frame. See below for an example use case
        /// to detect keypress via custom configuration.
        /// </summary>
        // TODO - Add your code here or remove this section if not required.
        private void Update()
        {
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
				Hof2_MinigamesCheatsPlugin.Log.LogInfo(msg);
			}
		}
    }
}
