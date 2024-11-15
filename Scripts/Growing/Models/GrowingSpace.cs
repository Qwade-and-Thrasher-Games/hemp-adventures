using System;
using Godot;
using HempAdventures.Scripts.Resources.Interfaces;


public partial class GrowingSpace : IGrowingSpace
{
    public string StrainName { get; private set; }
    public bool ReadyToHarvest { get; private set; }
    public Guid Guid { get; } = Guid.NewGuid();
    public bool IsLocked { get; private set; }
    public bool IsGrowing => !string.IsNullOrEmpty(StrainName);

    private readonly Button _newPlantButton;
    private readonly TextureRect _lockedOverlay;
    private readonly Label _strainLabel;
    private readonly Label _progressLabel;
    private readonly Timer _growingTimer;
    private readonly TextureRect _budOverlay;

    public GrowingSpace(string strainName, Panel panel)
    {
        StrainName = strainName;
        ReadyToHarvest = false;

        _newPlantButton = panel.GetNode<Button>("%NewPlantButton");
        _lockedOverlay = panel.GetNode<TextureRect>("%LockOverlay");
        _strainLabel = panel.GetNode<Label>("%StrainLabel");
        _progressLabel = panel.GetNode<Label>("%ProgressLabel");
        _growingTimer = panel.GetNode<Timer>("%GrowingTimer");
        _budOverlay = panel.GetNode<TextureRect>("%BudOverlay");

        UpdateProgress(0);
        _strainLabel.Text = "???";

        _growingTimer.WaitTime = _growingTimer.WaitTime / Constants.Game.GAME_SPEED;
    }

    public void Harvest()
    {
        ReadyToHarvest = false;
    }

    public void Plant(string strainName)
    {
        StrainName = strainName;
        _strainLabel.Text = TrimStrainName(strainName);
        if (_strainLabel.Text.Length > 15)
        {
            _strainLabel.TooltipText = strainName;
        }
        _newPlantButton.Visible = false;
        _budOverlay.Visible = true;
        _growingTimer.Timeout += OnGrowingTimerTimeout;
        _growingTimer.Start();
    }

    public void DestroyPlant() // MAYBE: Might do some sort of police raid or something so this could be useful later to destroy the plant
    {
        StrainName = string.Empty;
        _strainLabel.Text = "???";
        _budOverlay.Visible = false;
        _newPlantButton.Visible = true;
        if (!_growingTimer.IsStopped())
        {
            _growingTimer.Stop();
        }
    }

    public void Unlock()
    {
        IsLocked = false;
        _newPlantButton.Visible = true;
        _lockedOverlay.Visible = false;
    }
    public void Lock()
    {
        IsLocked = true;
        _newPlantButton.Visible = false;
        _lockedOverlay.Visible = true;
        _strainLabel.Text = "???";
        if (!_growingTimer.IsStopped())
        {
            _growingTimer.Stop();
        }
    }

    public void UpdateProgress(float progress)
    {
        _progressLabel.Text = $"Progress: {progress * 100}%";
    }

    private void OnGrowingTimerTimeout()
    {
        IResourceManager resourceManager = (IResourceManager)HempAdventures.Scripts.Core.GameManager.Instance.ResourceManager;

        if (resourceManager == null)
        {
            return;
        }

        resourceManager.AddResource(ResourceType.Money, 100, "grow payout");
    }

    private string TrimStrainName(string strainName)
    {
        if (strainName.Length > 15)
        {
            return strainName.Substring(0, 15) + "...";
        }
        return strainName;
    }
}