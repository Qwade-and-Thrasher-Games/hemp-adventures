
namespace HempAdventures.Scripts.Inventory.Interfaces;

public interface IInventoryChangeEventArgs
{
    string ItemName { get; }
    int NewQuantity { get; }
}