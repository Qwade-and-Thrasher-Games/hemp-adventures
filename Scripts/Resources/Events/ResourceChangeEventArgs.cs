using HempAdventures.Scripts.Resources.Interfaces;

namespace HempAdventures.Scripts.Resources.Events;

public class ResourceChangeEventArgs : IResourceChangeEventArgs
{
    public ResourceType Type { get; }
    public decimal Amount { get; }
    public string Source { get; }

    public ResourceChangeEventArgs(
        ResourceType type,
        decimal amount,
        string source)
    {
        Type = type;
        Amount = amount;
        Source = source;
    }
}