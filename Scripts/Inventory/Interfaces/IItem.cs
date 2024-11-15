namespace HempAdventures.Scripts.Inventory.Interfaces;

public interface IItem
{
    public string Name { get; }
    public string ItemImagePath { get; }
    public string ItemDescription { get; }
    public bool ItemIsStackable { get; }

    public void UseItem();
}