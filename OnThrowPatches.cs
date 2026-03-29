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

                if (!((interaction is BoxInteraction) || (interaction is FurnitureBoxInteraction)))
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


    [HarmonyPatch(typeof(FurniturePlacingMode), "PlacingMode")]
    internal static class Patch_FurniturePlacingMode_PlacingMode
    {
        [HarmonyPostfix]
        private static void Postfix(FurniturePlacingMode __instance, bool value)
        {
            try
            {
                // true = entering placement mode
                // false = leaving placement mode
                if (value)
                    return;

                if (__instance == null)
                    return;

                PlayerObjectHolder holder = Object.FindObjectOfType<PlayerObjectHolder>();
                if (holder == null)
                    return;

                if (holder.CurrentObject == null)
                    return;

                holder.SetNullCurrentObject();
            }
            catch (System.Exception ex)
            {
                Plugin.Log.LogError("[MultiBox] FurniturePlacingMode postfix error: " + ex);
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

    [HarmonyPatch(typeof(FurnitureBoxInteraction), "OnThrow")]
    internal static class FurnitureBoxInteraction_OnThrow_Patch
    {
        private static bool Prefix(bool isDown)
        {
            return isDown;
        }
    }

}