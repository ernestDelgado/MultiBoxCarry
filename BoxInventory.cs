using System.Collections.Generic;

namespace MultiBoxCarry
{
    internal class BoxInventory
    {
        public const int MaxQueuedBoxes = 5;

        private readonly List<Box> _queuedBoxes = new List<Box>();

        public int Count => _queuedBoxes.Count;
        public bool IsFull => _queuedBoxes.Count >= MaxQueuedBoxes;
        public bool IsEmpty => _queuedBoxes.Count == 0;
        public IReadOnlyList<Box> QueuedBoxes => _queuedBoxes;

        public bool Enqueue(Box box)
        {
            if (box == null)
                return false;

            if (IsFull)
                return false;

            if (_queuedBoxes.Contains(box))
                return false;

            _queuedBoxes.Add(box);
            return true;
        }
        public Box Dequeue()
        {
            if (_queuedBoxes.Count == 0)
                return null;

            int lastIndex = _queuedBoxes.Count - 1;
            Box box = _queuedBoxes[lastIndex];
            _queuedBoxes.RemoveAt(lastIndex);
            return box;
        }

        public Box Peek()
        {
            if (_queuedBoxes.Count == 0)
                return null;

            return _queuedBoxes[_queuedBoxes.Count - 1];
        }
        public bool Remove(Box box)
        {
            if (box == null)
                return false;

            return _queuedBoxes.Remove(box);
        }

        public void Clear()
        {
            _queuedBoxes.Clear();
        }
    }
}