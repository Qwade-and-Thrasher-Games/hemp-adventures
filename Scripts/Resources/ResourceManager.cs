using Godot;
using System;
using System.Collections.Generic;
using HempAdventures.Scripts.Resources.Events;
using HempAdventures.Scripts.Resources.Interfaces;

namespace HempAdventures.Scripts.Resources;
public partial class ResourceManager : Node, IResourceManager
{
    #region Events
    public event Action<IResourceChangeEventArgs> OnResourceChanged;
    #endregion

    #region Private Fields
    private readonly Dictionary<ResourceType, decimal> _resources;
    #endregion

    #region constructor
    public ResourceManager()
    {
        _resources = new Dictionary<ResourceType, decimal>();
    }
    #endregion

    public override void _Ready()
    {
        InitializeResources();
    }

    private void InitializeResources()
    {
        foreach (ResourceType type in Enum.GetValues(typeof(ResourceType)))
        {
            _resources[type] = 0;
        }
    }

    public decimal GetResource(ResourceType type)
    {
        return _resources.GetValueOrDefault(type, 0);
    }

    public bool HasSufficient(ResourceType type, decimal amount)
    {
        return GetResource(type) >= amount;
    }

    public bool TrySpendResource(ResourceType type, decimal amount, string source)
    {
        if (!HasSufficient(type, amount))
        {
            GD.PrintErr($"Insufficient {type} to spend {amount}");
            return false;
        }
        _resources[type] -= amount;
        OnResourceChanged?.Invoke(new ResourceChangeEventArgs(type, -amount, source));
        return true;
    }

    public void AddResource(ResourceType type, decimal amount, string source)
    {
        _resources[type] += amount;
        OnResourceChanged?.Invoke(new ResourceChangeEventArgs(type, amount, source));
    }

    public Dictionary<ResourceType, decimal> GetResources()
    {
        return _resources;
    }
}