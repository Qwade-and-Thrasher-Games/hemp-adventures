using Godot;

public static class ResourceUtils
{
    public static string FormatAmount(decimal amount)
    {
        return amount >= 1000000
            ? $"{amount / 1000000:N1}M"
            : amount >= 1000
                ? $"{amount / 1000:N1}K"
                : amount.ToString("N0");
    }

    public static Color GetResourceColor(ResourceType type)
    {
        return type switch
        {
            ResourceType.Money => Colors.Gold,
            ResourceType.Seeds => Colors.Green,
            ResourceType.Water => Colors.Blue,
            ResourceType.Energy => Colors.Yellow,
            ResourceType.ResearchPoints => Colors.Purple,
            _ => Colors.White
        };
    }
}