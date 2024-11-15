using Godot;
using HempAdventures.Scripts.Inventory.Interfaces;

namespace HempAdventures.Scripts.Inventory.Models;

public class Item : IItem
{
    public string Name { get; private set; }
    public string ItemImagePath { get; private set; }
    public string ItemDescription { get; private set; }
    public bool ItemIsStackable { get; private set; }

    public Item(string name, string itemImagePath, string itemDescription, bool itemIsStackable  = true)
    {
        Name = name;
        ItemImagePath = itemImagePath;
        ItemDescription = itemDescription;
        ItemIsStackable = itemIsStackable;
    }

    public virtual void UseItem()
    {
        GD.Print($"{Name} has no UseItem method implemented.");
    }
}