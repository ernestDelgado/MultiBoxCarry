namespace MultiBoxCarry
{
    internal static class PlayerInventoryManager
    {
        public static BoxInventory Inventory { get; private set; } = new BoxInventory();

        public static void Reset()
        {
            Inventory = new BoxInventory();
        }
    }
}