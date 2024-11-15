using HempAdventures.Scripts.Inventory.Interfaces;

namespace HempAdventures.Scripts.Inventory.Events;

public class InventoryChangeEventArgs : IInventoryChangeEventArgs
{
    public string ItemName { get; set; }
    public int NewQuantity { get; set; }
    
    public InventoryChangeEventArgs(string itemName, int newQuantity)
    {
        ItemName = itemName;
        NewQuantity = newQuantity;
    }
}