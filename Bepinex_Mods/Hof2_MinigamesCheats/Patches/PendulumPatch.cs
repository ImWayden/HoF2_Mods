using HarmonyLib;

namespace Hof2_MinigamesCheats.Patches
{
    // TODO Review this file and update to your own requirements, or remove it altogether if not required

    /// <summary>
    /// Sample Harmony Patch class. Suggestion is to use one file per patched class
    /// though you can include multiple patch classes in one file.
    /// Below is included as an example, and should be replaced by classes and methods
    /// for your mod.
    /// </summary>
    [HarmonyPatch(typeof(Pendulum))]
    internal class PendulumPatch
	{
        /// <summary>
        /// Patches the Player Awake method with prefix code.
        /// </summary>
        /// <param name="__instance"></param>
        [HarmonyPatch(nameof(Pendulum.Evaluate))]
        [HarmonyPrefix]
        public static bool Evaluate_Prefix(ref ChanceType __result)
        {
			Hof2_MinigamesCheatsPlugin.Log.LogInfo("In Pendulum prefix.");
			if (Hof2_MinigamesCheatsPlugin.PendulumCheat)
			{
				__result = ChanceType.HugeSuccess;
				return false;
			}
            return true;
        }
    }
}