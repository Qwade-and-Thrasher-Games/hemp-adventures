using System;
using System.Collections.Generic;

namespace HempAdventures.Scripts.Resources.Interfaces;

public interface IResourceManager
{
    decimal GetResource(ResourceType type);
    bool HasSufficient(ResourceType type, decimal amount);
    bool TrySpendResource(ResourceType type, decimal amount, string source);
    void AddResource(ResourceType type, decimal amount, string source);
    Dictionary<ResourceType, decimal> GetResources();
    event Action<IResourceChangeEventArgs> OnResourceChanged;
}