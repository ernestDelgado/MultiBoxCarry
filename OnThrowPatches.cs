using HarmonyLib;
using UnityEngine;
using UnityEngine.InputSystem;

namespace MultiBoxCarry
{

    [HarmonyPatch(typeof(PlayerObjectHolder), "TryThrowObject")]
    internal static class OnThrowAttempt
    {
        [HarmonyPostfix]
        public static void Postfix(bool __result)
        {
            try
            {
                if (!__result) return;
                OnThrowMessanger.WriteMessage();
            }
            catch (System.Exception)
            {
            }
        }
    }

    // SUCCESS: On Throw Success
    [HarmonyPatch(typeof(PlayerInteraction), "InteractionEnd")]
    internal static class OnThrowSuccess
    {
        [HarmonyPostfix]
        private static void Postfix(PlayerInteraction __instance, Interaction interaction)
        {
            try
            {
                if (!OnThrowMessanger.hasMessage)
                    return;

                if (__instance == null || interaction == null)
                    return;

                if (!(interaction is BoxInteraction))
                    return;

                PlayerObjectHolder player = __instance.GetComponent<PlayerObjectHolder>();
                player.SetNullCurrentObject();

                OnThrowMessanger.GaveMessage();
            }
            catch (System.Exception ex)
            {
                Plugin.Log.LogError("[InteractionEndPatch] Error: " + ex);
            }
        }
    }

    [HarmonyPatch(typeof(BoxInteraction), "OnThrow")]
    internal static class BoxInteraction_OnThrow_Patch
    {
        private static bool Prefix(bool isDown)
        {
            return isDown;
        }
    }

}