using System;
using System.Collections.Generic;

public interface IFacilityManager
{
    void AddFacility(IFacility facility);
    void RemoveFacility(IFacility facility);
    IFacility GetFacility(Guid id);
    Dictionary<Guid, IFacility> GetFacilities();
    event Action<IFacility> OnFacilityAdded;
    event Action<IFacility> OnFacilityRemoved;
}
