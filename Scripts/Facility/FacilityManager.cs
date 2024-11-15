using Godot;
using System;
using System.Collections.Generic;

public partial class FacilityManager : Node, IFacilityManager
{
    #region Events
    public event Action<IFacility> OnFacilityAdded;
    public event Action<IFacility> OnFacilityRemoved;
    #endregion

    #region Private Fields
    private readonly Dictionary<Guid, IFacility> _facilities;
    #endregion

    public IFacility SelectedFacility { get; set; }

    #region Constructor
    public FacilityManager()
    {
        _facilities = new Dictionary<Guid, IFacility>();
    }

    public override void _Ready()
    {
        InitializeFacilities();
    }

    private void InitializeFacilities()
    {
        GD.Print("FacilityManager Ready!");
    }

    public void AddFacility(IFacility facility)
    {
        if (_facilities.ContainsKey(facility.Id))
        {
            GD.PrintErr($"Facility with ID {facility.Id} already exists!");
            return;
        }
        _facilities.Add(facility.Id, facility);
        OnFacilityAdded?.Invoke(facility);
    }

    public void RemoveFacility(IFacility facility)
    {
        if (!_facilities.ContainsKey(facility.Id))
        {
            GD.PrintErr($"Facility with ID {facility.Id} does not exist!");
            return;
        }
        _facilities.Remove(facility.Id);
        OnFacilityRemoved?.Invoke(facility);
    }

    public IFacility GetFacility(Guid id)
    {
        return _facilities.GetValueOrDefault(id);
    }

    public Dictionary<Guid, IFacility> GetFacilities()
    {
        return _facilities;
    }

    public void SelectFacility(Guid id)
    {
        SelectedFacility = GetFacility(id);
    }

    #endregion

    public override void _ExitTree()
    {
        _facilities.Clear();
    }
}