using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace MultiBoxCarry
{
    internal class FurnitureBoxAdapter : IQueuableBox 
    {
        private readonly FurnitureBox _furnitureBox;
        public FurnitureBoxAdapter(FurnitureBox furnitureBox)
        {
            _furnitureBox = furnitureBox;
        }

        public object Raw => _furnitureBox;
        public Transform transform => _furnitureBox.transform;
        public int GetID() => _furnitureBox.GetComponent<FurnitureBoxData>().FurnitureID;
        

        public void Drop(PlayerInteraction player)
        {
            FurnitureBoxInteraction furnitureBoxInteraction = player.GetComponent<FurnitureBoxInteraction>();
            if (furnitureBoxInteraction == null)
                return;

            furnitureBoxInteraction.DropBox(this.transform.position);
        }
        public void HideAndAttach(Transform player, Vector3 offset)
        {
            BoxUtility.HideAndAttachShared(player, this, offset);
        }
        public bool IsOccupied() => false;
        public void Restore(Transform playerTransform)
        {
            BoxUtility.RestoreShared(playerTransform, this);
        }

        public FurnitureBox GetFurnitureBox() => _furnitureBox;
    }
}
