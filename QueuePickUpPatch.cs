using System;
using System.Linq;
using HarmonyLib;
using UnityEngine;
using UnityEngine.InputSystem;

namespace MultiBoxCarry
{
    [HarmonyPatch(typeof(PlayerInteraction), "OnUse")]
    internal static class QueuePickUpPatch
    {
        private const float SWAP_RAY_DISTANCE = 2.5f;

        [HarmonyPrefix]
        private static bool Prefix(PlayerInteraction __instance, InputAction.CallbackContext context)
        {
            try
            {
                bool pressed = false;
                try
                {
                    pressed = context.started;
                }
                catch (Exception ex)
                {
                    Plugin.Log.LogWarning("[MultiBox] Could not read CallbackContext flags: " + ex);
                }

                if (!pressed)
                    return true;

                PlayerObjectHolder holder = __instance.GetComponent<PlayerObjectHolder>();
                if (holder == null)
                {
                    Plugin.Log.LogInfo("[MultiBox] PlayerObjectHolder not found. Letting vanilla continue.");
                    return true;
                }

                Camera cam = __instance.m_MainCamera;
                if (cam == null)
                {
                    Plugin.Log.LogWarning("[MultiBox] Camera was null. Letting vanilla continue.");
                    return true;
                }

                IQueuableBox heldBox = GetHeldQueueBox(holder);
                if (heldBox == null)
                    return true;

                if (IsInPlacingMode(__instance))
                    return true;

                IQueuableBox targetBox = FindTargetQueueBox(cam, __instance, heldBox);
                if (targetBox == null)
                    return true;

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
                Plugin.Log.LogError("[MultiBox] QueuePickUpPatch Prefix error: " + ex);
                return true;
            }
        }

        

        private static IQueuableBox FindTargetQueueBox(Camera cam, PlayerInteraction player, IQueuableBox heldBox)
        {
            Ray ray = new Ray(cam.transform.position, cam.transform.forward);
            RaycastHit[] hits = Physics.RaycastAll(ray, SWAP_RAY_DISTANCE, ~0, QueryTriggerInteraction.Collide);

            if (hits == null || hits.Length == 0)
                return null;

            foreach (RaycastHit hit in hits.OrderBy(h => h.distance))
            {
                if (hit.collider == null)
                    continue;

                Transform hitTransform = hit.collider.transform;
                if (hitTransform == null)
                    continue;

                if (hitTransform.IsChildOf(player.transform))
                    continue;

                if (hit.collider.CompareTag("Player"))
                    continue;

                Transform parent = hitTransform.parent;
                if (parent != null && parent.name.Contains("Rack Manager"))
                    break;

                IQueuableBox hitQueueBox = GetQueueBoxFromCollider(hit.collider);
                if (hitQueueBox == null)
                    continue;

                if (AreSameUnderlyingObject(hitQueueBox, heldBox))
                    continue;

                if (IsQueueBoxOccupied(hitQueueBox))
                    continue;

                return hitQueueBox;
            }

            return null;
        }

        private static IQueuableBox GetHeldQueueBox(PlayerObjectHolder holder)
        {
            if (holder == null || holder.CurrentObject == null)
                return null;

            GameObject heldObject = holder.CurrentObject.TryCast<GameObject>();
            if (heldObject == null)
                return null;

            Box heldBox = heldObject.GetComponent<Box>();
            if (heldBox != null)
                return new BoxAdapter(heldBox);

            FurnitureBox heldFurnitureBox = heldObject.GetComponent<FurnitureBox>();
            if (heldFurnitureBox != null)
                return new FurnitureBoxAdapter(heldFurnitureBox);

            return null;
        }

        private static IQueuableBox GetQueueBoxFromCollider(Collider collider)
        {
            if (collider == null)
                return null;

            Box hitBox = collider.GetComponentInParent<Box>();
            if (hitBox != null)
                return new BoxAdapter(hitBox);

            FurnitureBox hitFurnitureBox = collider.GetComponentInParent<FurnitureBox>();
            if (hitFurnitureBox != null)
                return new FurnitureBoxAdapter(hitFurnitureBox);

            return null;
        }

        private static bool AreSameUnderlyingObject(IQueuableBox a, IQueuableBox b)
        {
            if (a == null || b == null)
                return false;

            return ReferenceEquals(a.Raw, b.Raw);
        }

        private static bool IsQueueBoxOccupied(IQueuableBox queueBox)
        {
            if (queueBox == null)
                return false;

            return queueBox.IsOccupied();
        }

        private static bool IsInPlacingMode(PlayerInteraction player)
        {
            if (player == null)
                return false;

            BoxInteraction boxInteraction = player.GetComponent<BoxInteraction>();
            if (boxInteraction != null && boxInteraction.m_PlacingMode)
                return true;

            FurnitureBoxInteraction furnitureInteraction = player.GetComponent<FurnitureBoxInteraction>();
            if (furnitureInteraction != null && furnitureInteraction.m_PlacingMode)
                return true;

            return false;
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
            catch (Exception ex)
            {
                Plugin.Log.LogError("[WarningHelper] " + ex);
            }
        }
    }
}