using System;

public interface IGrowingSpace
{
    Guid Guid { get; }
    bool IsGrowing { get; }
    bool ReadyToHarvest { get; }
    bool IsLocked { get; }
    string StrainName { get; }
    void Plant(string strainName);
    void DestroyPlant();
    void Harvest();
    void Unlock();
    void Lock();
    void UpdateProgress(float progress);
}