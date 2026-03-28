using UnityEngine;

namespace MultiBoxCarry
{
    internal static class BoxUtility
    {
        public static void HideAndAttachBox(Transform playerTransform, Box box, Vector3 localOffset)
        {
            if (playerTransform == null || box == null)
                return;

            box.SetOccupy(true, playerTransform);
            SetBoxVisible(box, false);
            SetBoxColliders(box, false);
            SetBoxPhysicsQueued(box);
            box.transform.SetParent(playerTransform, false);
            box.transform.localPosition = localOffset;
            box.transform.localRotation = Quaternion.identity;
        }

        public static void RestoreBox(Box box, Transform playerTransform)
        {
            if (box == null)
                return;

            box.SetOccupy(false, playerTransform);
            SetBoxVisible(box, true);
            SetBoxColliders(box, true);
            SetBoxPhysicsWorld(box);

            box.transform.SetParent(null, true);
        }

        public static void SetBoxVisible(Box box, bool visible)
        {
            if (box == null)
                return;

            foreach (var renderer in box.GetComponentsInChildren<Renderer>(true))
            {
                if (renderer != null)
                    renderer.enabled = visible;
            }
        }

        public static void SetBoxColliders(Box box, bool enabled)
        {
            if (box == null)
                return;

            foreach (var collider in box.GetComponentsInChildren<Collider>(true))
            {
                if (collider != null)
                    collider.enabled = enabled;
            }
        }

        public static void SetBoxPhysicsQueued(Box box)
        {
            if (box == null)
                return;

            var rb = box.GetComponent<Rigidbody>();
            if (rb == null)
                return;

            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic = true;
            rb.detectCollisions = false;
            rb.interpolation = RigidbodyInterpolation.None;
        }

        public static void SetBoxPhysicsWorld(Box box)
        {
            if (box == null)
                return;

            var rb = box.GetComponent<Rigidbody>();
            if (rb == null)
                return;

            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic = false;
            rb.detectCollisions = true;
        }

 

        public static Vector3 GetQueueLocalOffset(int index)
        {
            return new Vector3(0f, 0.45f, 0f + (0.18f * index));
        }
    }
}