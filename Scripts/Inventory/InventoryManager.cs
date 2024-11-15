using System;
using System.Collections.Generic;
using Godot;
using HempAdventures.Scripts.Inventory.Events;
using HempAdventures.Scripts.Inventory.Interfaces;
using HempAdventures.Scripts.Inventory.Models;

namespace HempAdventures.Scripts.Inventory;

public partial class InventoryManager : Node
{
    private Dictionary<string, (Item Item, int Quantity)> _items = new();

    public event Action<IInventoryChangeEventArgs> OnInventoryChanged;

    public void AddItem(Item item, int quantity = 1)
    {
        if (quantity <= 0) return;
        int newQuantity = quantity;

        if (_items.ContainsKey(item.Name))
        {
            var (existing, currentQuantity) = _items[item.Name];
            newQuantity += currentQuantity;
            _items[item.Name] = (existing, currentQuantity + quantity);
        }
        else
        {
            _items[item.Name] = (item, quantity);
        }
        
        OnInventoryChanged?.Invoke(new InventoryChangeEventArgs(
                item.Name,
                newQuantity
            ));
    }

    public bool RemoveItem(string itemName, int quantity = 1)
    {
        if (!_items.ContainsKey(itemName) || quantity <= 0) return false;
        
        var (existing, currentQuantity) = _items[itemName];
        
        var newQuantity = currentQuantity - quantity;
        if (newQuantity <= 0)
        {
            _items.Remove(itemName);
        }
        else
        {
            _items[itemName] = (existing, newQuantity);
        }
        
        OnInventoryChanged?.Invoke(new InventoryChangeEventArgs(
            existing.Name,
            newQuantity
        ));
        return true;
    }

    public int GetItemQuantity(string itemName)
    {
        return _items.ContainsKey(itemName) ? _items[itemName].Quantity : 0;
    }

    public Dictionary<string, (Item Item, int Quantity)> GetItems()
    {
        return new Dictionary<string, (Item Item, int Quantity)>(_items);
    }

    public bool UseItem(string itemName)
    {
        if (!_items.ContainsKey(itemName)) return false;
        
        var (existing, currentQuantity) = _items[itemName];
        existing.UseItem();
        
        return RemoveItem(itemName, 1);
    }

    public bool HasItem(string itemName, int quantity = 1)
    {
        return _items.ContainsKey(itemName) && _items[itemName].Quantity >= quantity;
    }
}