using UnityEngine;
using UnityEngine.InputSystem;

namespace MultiBoxCarry
{
    internal static class BoxInventoryController
    {
        public static bool TryQueueBox(PlayerInteraction player, Box heldBox, Box targetBox)
        {
            if (player == null || heldBox == null || targetBox == null)
                return true;

            BoxInventory inventory = PlayerInventoryManager.Inventory;
            if (inventory == null || inventory.IsFull)
            {
                Plugin.Log.LogInfo("[InventoryController] Queue full.");
                return true;
            }

            int queueIndex = inventory.Count;

            // returns RackSlot of Box, otherwise Null 
            var rackSlot = GetRackSlot(targetBox);

            // RACK CASE
            if (rackSlot != null)
            {
                bool sameProduct = IsSameProduct(heldBox, targetBox);
                bool leftShiftHeld = Keyboard.current != null && Keyboard.current.leftShiftKey.isPressed;

                if (sameProduct && !leftShiftHeld)
                {
                    return false; // let vanilla handle
                }

                player.GetComponent<BoxInteraction>().DropBox();
                inventory.Enqueue(heldBox);
                BoxUtility.HideAndAttachBox(player.transform, heldBox, BoxUtility.GetQueueLocalOffset(queueIndex));
                rackSlot.InstantInteract();

                return true; // block Vanilla
            }

            // WORLD CASE
            player.GetComponent<BoxInteraction>().DropBox();
            inventory.Enqueue(heldBox);
            BoxUtility.HideAndAttachBox(player.transform, heldBox, BoxUtility.GetQueueLocalOffset(queueIndex));
            player.SetCurrentInteractable(targetBox.GetComponent<IInteractable>());
            player.Interact();

            return true; // block vanilla
        }

        public static bool TryPromoteNextBox(PlayerInteraction player)
        {
            if (player == null)
                return true;

            BoxInventory inventory = PlayerInventoryManager.Inventory;
            if (inventory == null || inventory.IsEmpty)
                return true;

            Box nextBox = inventory.Dequeue();
            if (nextBox == null)
                return true;

            BoxUtility.RestoreBox(nextBox);

            IInteractable interactable = nextBox.GetComponent<IInteractable>();
            if (interactable == null)
            {
                return true;
            }

            player.SetCurrentInteractable(interactable);
            player.Interact();

            ReflowQueuedBoxes(player);

            return false;
        }

        public static void ReflowQueuedBoxes(PlayerInteraction player)
        {
            if (player == null)
                return;

            BoxInventory inventory = PlayerInventoryManager.Inventory;
            if (inventory == null || inventory.IsEmpty)
                return;

            for (int i = 0; i < inventory.QueuedBoxes.Count; i++)
            {
                Box box = inventory.QueuedBoxes[i];
                if (box == null)
                    continue;

                box.transform.SetParent(player.transform, false);
                box.transform.localPosition = BoxUtility.GetQueueLocalOffset(i);
                box.transform.localRotation = Quaternion.identity;
            }
        }

        private static RackSlot GetRackSlot(Box box)
        {
            if (box == null)
                return null;
            
            var parent = box.transform.parent;
            if (parent == null)
                return null;

            var rackSlot = parent.GetComponent<RackSlot>();

            return rackSlot;
        }

        private static bool IsSameProduct(Box heldBox, Box targetBox)
        {
            if (heldBox == null || targetBox == null)
                return false;

            if (heldBox.Product == null || targetBox.Product == null)
                return false;

            return heldBox.Product == targetBox.Product;
        }
    }
}