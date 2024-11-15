namespace HempAdventures.Scripts.Resources.Interfaces;

public interface IResourceChangeEventArgs
{
    ResourceType Type { get; }
    decimal Amount { get; }
    string Source { get; }
}