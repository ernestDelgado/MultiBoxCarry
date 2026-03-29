using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace MultiBoxCarry
{
    internal interface IQueuableBox
    {
        Transform transform { get; }
        object Raw {  get; }

        void HideAndAttach(Transform playerTransform, Vector3 offset);
        void Restore(Transform playerTransform);
        bool IsOccupied();
        void Drop(PlayerInteraction player);
        int GetID();
    }
}
