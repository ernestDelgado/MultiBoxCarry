using UnityEngine;
using static HarmonyLib.Code;

namespace MultiBoxCarry
{
    internal static class BoxUtility
    {
        public static void HideAndAttachBox(Transform playerTransform, IQueuableBox queueBox, Vector3 localOffset)
        {
            if (playerTransform == null || queueBox == null)
                return;

            queueBox.HideAndAttach(playerTransform, localOffset);
            
        }

        internal static void HideAndAttachShared(Transform playerTransform, IQueuableBox queueBox, Vector3 localOffset)
        {
            SetBoxVisible(queueBox, false);
            SetBoxColliders(queueBox, false);
            SetBoxPhysicsQueued(queueBox);
            queueBox.transform.SetParent(playerTransform, false);
            queueBox.transform.localPosition = localOffset;
            queueBox.transform.localRotation = Quaternion.identity;
        }

        public static void RestoreBox(IQueuableBox queueBox, Transform playerTransform)
        {
            if (queueBox == null)
                return;

            queueBox.Restore(playerTransform);
        }

        internal static void RestoreShared(Transform playerTransform, IQueuableBox queueBox)
        {
            queueBox.transform.SetParent(null, true);
            SetBoxVisible(queueBox, true);
            SetBoxColliders(queueBox, true);
            SetBoxPhysicsWorld(queueBox);
        }

        public static void SetBoxVisible(IQueuableBox queueBox, bool visible)
        {
            if (queueBox == null)
                return;

            foreach (var renderer in queueBox.transform.gameObject.GetComponentsInChildren<Renderer>(true))
            {
                if (renderer != null)
                    renderer.enabled = visible;
            }
        }

        public static void SetBoxColliders(IQueuableBox queueBox, bool enabled)
        {
            if (queueBox == null)
                return;

            foreach (var collider in queueBox.transform.GetComponentsInChildren<Collider>(true))
            {
                if (collider != null)
                    collider.enabled = enabled;
            }
        }

        public static void SetBoxPhysicsQueued(IQueuableBox queueBox)
        {
            if (queueBox == null)
                return;

            var rb = queueBox.transform.GetComponent<Rigidbody>();
            if (rb == null)
                return;

            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic = true;
            rb.detectCollisions = false;
            rb.interpolation = RigidbodyInterpolation.None;
        }

        public static void SetBoxPhysicsWorld(IQueuableBox queueBox)
        {
            if (queueBox == null)
                return;

            var rb = queueBox.transform.GetComponent<Rigidbody>();
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