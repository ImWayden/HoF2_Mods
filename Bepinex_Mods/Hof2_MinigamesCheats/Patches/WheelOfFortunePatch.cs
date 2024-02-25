using HarmonyLib;
using System;
using System.Runtime.CompilerServices;

namespace Hof2_MinigamesCheats.Patches
{
	[HarmonyPatch(typeof(WheelOfFortune))]
	internal class WheelOfFortunePatch
    {
		/// <summary>
		/// Patches the Player Awake method with prefix code.
		/// </summary>
		/// <param name="__instance"></param>
		[HarmonyPatch(nameof(WheelOfFortune.CreateChanceCard))]
        [HarmonyPrefix]
        public static bool OnActivate_Prefix(ref ChanceType a_chanceType)
        {
			if (Hof2_MinigamesCheatsPlugin.WheelOFFortuneCheat)
				a_chanceType = ChanceType.HugeSuccess;
            return true;
        }
    }

	[HarmonyPatch(typeof(WheelSpinTask))]
	internal class WheelSpinTaskPatch
	{

		[HarmonyPatch(nameof(WheelSpinTask.Spin))]
		[HarmonyPrefix]
		public static bool Spin_prefix(WheelSpinTask __instance)
		{
			if (Hof2_MinigamesCheatsPlugin.WheelOFFortuneCheat)
				__instance.m_difficulty = WheelDifficulty.Easy;
			return true;
		}
	}
}