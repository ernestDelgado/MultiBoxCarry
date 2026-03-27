using HarmonyLib;
using UnityEngine;

namespace MultiBoxCarry
{
    [HarmonyPatch(typeof(PlayerInteraction), "Update")]
    internal static class AutoRefillPatch
    {
        static void Postfix(PlayerInteraction __instance)
        {
            try
            {
                if (__instance == null)
                    return;

                var holder = __instance.GetComponent<PlayerObjectHolder>();
                if (holder == null)
                    return;

                // Hands are empty
                if (holder.CurrentObject == null)
                {
                    var inventory = PlayerInventoryManager.Inventory;

                    // If we have queued boxes → promote next
                    if (!inventory.IsEmpty)
                    {
                        BoxInventoryController.TryPromoteNextBox(__instance);
                    }
                }
            }
            catch (System.Exception e)
            {
                Plugin.Log.LogError("[AutoRefillPatch] " + e);
            }
        }
    }
}