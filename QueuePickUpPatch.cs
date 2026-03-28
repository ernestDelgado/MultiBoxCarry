using System;
using System.Linq;
using System.Runtime.InteropServices;
using HarmonyLib;
using UnityEngine;
using UnityEngine.InputSystem;

namespace MultiBoxCarry
{
    [HarmonyPatch(typeof(PlayerInteraction), "OnUse")]
    internal static class QueuePickUpPatch
    {
        private const float SWAP_RAY_DISTANCE = 2.5f;

        //Fires on LMB
        [HarmonyPrefix]
        private static bool Prefix(PlayerInteraction __instance, InputAction.CallbackContext context)
        {
            try
            {

                //CHECK: Pressed Context Existance
                bool pressed = false;
                try { pressed = context.started; }
                catch (Exception ex) { Plugin.Log.LogWarning("[MultiBox] Could not read CallbackContext flags: " + ex); }
                if (!pressed)
                    return true;
                Plugin.Log.LogInfo("Pressed LMB");

                //CHECK: PlayerObjectHolder Component Existance
                PlayerObjectHolder holder = __instance.GetComponent<PlayerObjectHolder>();
                if (holder == null)
                {
                    Plugin.Log.LogInfo("[MultiBox] PlayerObjectHolder not found. Letting vanilla continue.");
                    return true;
                }

                //CHECK: Camera Existance
                Camera cam = Camera.main;
                if (cam == null)
                {
                    Plugin.Log.LogWarning("[MultiBox] Camera.main was null. Letting vanilla continue.");
                    return true;
                }

                //GET: Gets the heldBox in hands if any
                Box heldBox = GetHeldBox(holder);
                if (heldBox == null)
                {
                    return true;
                }

                BoxInteraction boxInteraction = __instance.GetComponent<BoxInteraction>();
                if (boxInteraction.m_PlacingMode)
                    return true;

                //CHECK: for a box
                var targetBox = FindTargetBox(cam, __instance, heldBox);
                if (targetBox == null) return true;

                BoxInventory inventory = PlayerInventoryManager.Inventory;
                if (inventory == null)
                    return true;
                if (inventory.IsFull)
                {
                    ShowWarningMessage("Max Boxes Reached");
                    return true;
                }


                return !BoxInventoryController.TryQueueBox(__instance, heldBox, targetBox);

            }
            catch (Exception ex)
            {
                Plugin.Log.LogError("[MultiBox] Swap Prefix error: " + ex);
                return true;
            }
        }

        //RAYCAST:
        private static Box FindTargetBox(Camera cam, PlayerInteraction player, Box heldBox)
        {
            var ray = new Ray(cam.transform.position, cam.transform.forward);
            var hits = Physics.RaycastAll(ray, SWAP_RAY_DISTANCE, ~0, QueryTriggerInteraction.Collide);

            if (hits == null || hits.Length == 0)
                return null;

            foreach (var hit in hits.OrderBy(h => h.distance))
            {
                if (hit.collider == null)
                    continue;

                if (hit.collider.transform != null && hit.collider.transform.IsChildOf(player.transform))
                    continue;
                if (hit.collider.CompareTag("Player"))
                    continue;

                var parent = hit.collider.transform.parent;
                if (parent != null && parent.name.Contains("Rack Manager"))
                    break;

                var hitBox = hit.collider.GetComponentInParent<Box>();
                if (hitBox == null)
                    continue;

                if (hitBox == heldBox)
                    continue;

                if (hitBox.OccupyOwner != null)
                    continue;

                

                return hitBox;
            }

            return null;
        }

        private static Box GetHeldBox(PlayerObjectHolder holder)
        {
            if (holder.CurrentObject == null)
                return null;

            GameObject heldObject = holder.CurrentObject.TryCast<GameObject>();
            if (heldObject == null)
                return null;

            Box heldBox = heldObject.GetComponent<Box>();
            if (heldBox == null)
                return null;

            return heldBox;
        }

        public static void ShowWarningMessage(string message, float duration = 2f)
        {
            try
            {
                WarningCanvas warningCanvas = UnityEngine.Object.FindObjectOfType<WarningCanvas>();
                if (warningCanvas != null)
                {
                    warningCanvas.ShowInteractionWarningWithArgument(
                        InteractionWarningType.LIMIT_REACHED,
                        ""
                    );
                }
            }
            catch (System.Exception ex)
            {
                Plugin.Log.LogError("[WarningHelper] " + ex);
            }
        }


    }
}
