using HarmonyLib;

namespace Hof2_MinigamesCheats.Patches
{
	[HarmonyPatch(typeof(ChanceCardTask))]
	internal class ChanceCardTaskPatch
    {
		/// <summary>
		/// Patches the Player Awake method with prefix code.
		/// </summary>
		/// <param name="__instance"></param>
		[HarmonyPatch(nameof(ChanceCardTask.CreateChanceCard))]
        [HarmonyPrefix]
        public static bool OnActivate_Prefix(ref ChanceType a_chanceType)
        {
			if (Hof2_MinigamesCheatsPlugin.ChanceCardCheat)
				a_chanceType = ChanceType.HugeSuccess;
            return true;
        }
    }
}