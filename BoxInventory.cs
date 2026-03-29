using System.Collections.Generic;

namespace MultiBoxCarry
{
    internal class BoxInventory
    {
        public const int MaxQueuedBoxes = 5;

        private readonly List<IQueuableBox> _queuedBoxes = new List<IQueuableBox>();

        public int Count => _queuedBoxes.Count;
        public bool IsFull => _queuedBoxes.Count >= MaxQueuedBoxes;
        public bool IsEmpty => _queuedBoxes.Count == 0;
        public IReadOnlyList<IQueuableBox> QueuedBoxes => _queuedBoxes;

        public bool Enqueue(IQueuableBox box)
        {
            if (box == null)
                return false;

            if (IsFull)
                return false;

            foreach (IQueuableBox queued in _queuedBoxes)
            {
                if (queued != null && ReferenceEquals(queued.Raw, box.Raw))
                    return false;
            }

            _queuedBoxes.Add(box);
            return true;
        }

        public IQueuableBox Dequeue()
        {
            if (_queuedBoxes.Count == 0)
                return null;

            int lastIndex = _queuedBoxes.Count - 1;
            IQueuableBox box = _queuedBoxes[lastIndex];
            _queuedBoxes.RemoveAt(lastIndex);
            return box;
        }

        public IQueuableBox Peek()
        {
            if (_queuedBoxes.Count == 0)
                return null;

            return _queuedBoxes[_queuedBoxes.Count - 1];
        }
        public bool Remove(IQueuableBox box)
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