using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using static HarmonyLib.Code;

namespace MultiBoxCarry
{
    public class BoxAdapter : IQueuableBox
    {
        private readonly Box _box;
        public BoxAdapter(Box box)
        {
            _box = box;
        }

        public object Raw => _box;
        public Box GetBox() => _box;
        public int GetID() => _box.BoxID;
        public Transform transform => _box.transform;

        public void Drop(PlayerInteraction player)
        {
            BoxInteraction boxInteraction = player.GetComponent<BoxInteraction>();
            if (boxInteraction == null)
                return;

            boxInteraction.DropBox();
        }
        public void HideAndAttach(Transform player, Vector3 offset)
        {
            _box.SetOccupy(true, player); 
            BoxUtility.HideAndAttachShared(player, this, offset);
        }
        public bool IsOccupied()
        {
            if (_box.OccupyOwner != null)
                return true;
            return false;
        }
        public void Restore(Transform player)
        {
            _box.SetOccupy(false, player);
            BoxUtility.RestoreShared(player, this);
        }

        
    }
}
