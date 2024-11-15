using Godot;

namespace HempAdventures.Scripts.Inventory.Models;

public class Seed : Item
{
    public int GrowthTime { get; private set; }
    public float YieldAmount { get; private set; }

    public Seed(string seedName, string imagePath, int growthTime, float yieldAmount, string description = "")
        : base(seedName, imagePath, description)
    {
        GrowthTime = growthTime;
        YieldAmount = yieldAmount;
    }

    public override void UseItem()
    {
        GD.Print("Using Seed");
    }
}