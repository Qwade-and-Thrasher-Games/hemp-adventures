using System;
using System.Collections.Generic;

public interface IFacility
{
    string Name { get; }
    Guid Id { get; }
    FacilityType Type { get; }
    int MaxNumberOfGrowingSpaces { get; }
    Dictionary<Guid, IGrowingSpace> GrowingSpaces { get; }

    IGrowingSpace GetGrowingSpace(Guid guid);
    void AddGrowingSpace(IGrowingSpace growingSpace);
}