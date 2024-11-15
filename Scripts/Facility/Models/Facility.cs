using System;
using System.Collections.Generic;

public class Facility : IFacility
{
    public string Name { get; private set; }
    public Guid Id { get; private set; }
    public FacilityType Type { get; private set; }
    public int MaxNumberOfGrowingSpaces { get; private set; }
    public Dictionary<Guid, IGrowingSpace> GrowingSpaces { get; private set; } = new Dictionary<Guid, IGrowingSpace>();

    public Facility(string name, FacilityType type)
    {
        Name = name;
        Id = Guid.NewGuid();
        Type = type;
        MaxNumberOfGrowingSpaces = 10;
    }

    public IGrowingSpace GetGrowingSpace(Guid guid)
    {
        return GrowingSpaces[guid];
    }

    public void AddGrowingSpace(IGrowingSpace growingSpace)
    {
        GrowingSpaces.Add(growingSpace.Guid, growingSpace);
    }
}
