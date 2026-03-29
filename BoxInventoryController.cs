using UnityEngine;
using UnityEngine.InputSystem;

namespace MultiBoxCarry
{
    internal static class BoxInventoryController
    {
        public static bool TryQueueBox(PlayerInteraction player, IQueuableBox heldBox, IQueuableBox targetBox)
        {
            if (player == null || heldBox == null || targetBox == null)
                return false;

            BoxInventory inventory = PlayerInventoryManager.Inventory;
            if (inventory == null || inventory.IsFull)
            {
                Plugin.Log.LogInfo("[InventoryController] Queue full.");
                return false;
            }

            int queueIndex = inventory.Count;
            RackSlot rackSlot = GetRackSlot(targetBox);

            // RACK CASE
            if (rackSlot != null)
            {
                bool sameProduct = IsSameProduct(heldBox, targetBox);
                bool leftShiftHeld = Keyboard.current != null && Keyboard.current.leftShiftKey.isPressed;

                // Same product on shelf and no shift = let vanilla do its normal shelf swap/store behavior
                if (sameProduct && !leftShiftHeld)
                    return false;

                heldBox.Drop(player); //pass player for BoxInteraction or FurnitureInteraction

                if (!inventory.Enqueue(heldBox))
                    return false;

                BoxUtility.HideAndAttachBox(player.transform, heldBox, BoxUtility.GetQueueLocalOffset(queueIndex));

                // Let vanilla take the target box from the rack
                rackSlot.InstantInteract();

                return true; // block vanilla original logic
            }

            // WORLD CASE
            heldBox.Drop(player);

            if (!inventory.Enqueue(heldBox))
                return false;

            BoxUtility.HideAndAttachBox(player.transform, heldBox, BoxUtility.GetQueueLocalOffset(queueIndex));

            IInteractable interactable = targetBox.transform.GetComponent<IInteractable>();
            if (interactable != null)
            {
                player.SetCurrentInteractable(interactable);
                player.Interact();
            }

            return true; // block vanilla original logic
        }

        public static bool TryPromoteNextBox(PlayerInteraction player)
        {
            if (player == null)
                return true;

            BoxInventory inventory = PlayerInventoryManager.Inventory;
            if (inventory == null || inventory.IsEmpty)
                return true;

            IQueuableBox nextQueuedBox = inventory.Dequeue();
            if (nextQueuedBox == null)
                return true;

            BoxUtility.RestoreBox(nextQueuedBox, player.transform);

            IInteractable interactable = GetInteractable(nextQueuedBox);
            if (interactable == null)
            {
                ReflowQueuedBoxes(player);
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
                IQueuableBox queuedBox = inventory.QueuedBoxes[i];
                if (queuedBox == null)
                    continue;

                queuedBox.transform.SetParent(player.transform, false);
                queuedBox.transform.localPosition = BoxUtility.GetQueueLocalOffset(i);
                queuedBox.transform.localRotation = Quaternion.identity;
            }
        }

        private static IInteractable GetInteractable(IQueuableBox queuedBox)
        {
            if (queuedBox == null || queuedBox.Raw == null)
                return null;

            if (queuedBox.Raw is Component component)
                return component.GetComponent<IInteractable>();

            return null;
        }

        private static RackSlot GetRackSlot(IQueuableBox queueBox)
        {
            if (queueBox == null)
                return null;

            if (!(queueBox is BoxAdapter))
                return null;

            Transform parent = queueBox.transform.parent;
            if (parent == null)
                return null;

            return parent.GetComponent<RackSlot>();
        }

        private static bool IsSameProduct(IQueuableBox heldBox, IQueuableBox targetBox)
        {
            if (heldBox == null || targetBox == null)
                return false;

            BoxAdapter heldAdapter = heldBox as BoxAdapter;
            if (heldAdapter == null)
                return false;

            BoxAdapter targetAdapter = targetBox as BoxAdapter;
            if (targetAdapter == null)
                return false;

            Box held = heldAdapter.GetBox();
            Box target = targetAdapter.GetBox();

            if (held.Product == null || target.Product == null)
                return false;

            return held.Product == target.Product;
        }
    }
}