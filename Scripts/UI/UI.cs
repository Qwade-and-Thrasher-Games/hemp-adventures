using Godot;
using System;
using System.Collections.Generic;
using HempAdventures.Scripts.Resources.Interfaces;

public partial class UI : CanvasLayer
{
    [Export]
    public PackedScene GrowingSpacePanelScene;

    [Export]
    public PackedScene GrowingSpacePurchasePanel;

    [Export]
    public PackedScene FacilitySelectionPanel;

    private HempAdventures.Scripts.Core.GameManager _gameManager;
    private IResourceManager _resourceManager;
    private FacilityManager _facilityManager;

    private Label _moneyLabel;
    private Label _fpsLabel;
    private Label _gameSpeedLabel;
    private GridContainer _growingSpacesContainer;
    private Panel _growingSpacePurchasePanel;

    public override void _Ready()
    {
        _gameManager = GetTree().Root.GetNode<HempAdventures.Scripts.Core.GameManager>("GameManager");

        if (_gameManager != null)
        {
            _fpsLabel = GetNode<Label>("%FPSLabel");
            _gameSpeedLabel = GetNode<Label>("%GameSpeedLabel");

            _gameSpeedLabel.Text = $"Game Speed: {Constants.Game.GAME_SPEED}x";

            _growingSpacesContainer = GetNode<GridContainer>("%GrowingSpacesContainer");

            // Setup Resource Manager and subscribe to resource changed event
            _resourceManager = _gameManager.ResourceManager;
            _resourceManager.OnResourceChanged += OnResourceChanged;

            // Setup Resource Labels
            _moneyLabel = GetNode<Label>("%MoneyLabel");
            _moneyLabel.Text = "$" + ResourceUtils.FormatAmount(_resourceManager.GetResource(ResourceType.Money));

            // Initialize Facility Manager and create facility selection panels
            _facilityManager = _gameManager.FacilityManager;
            Dictionary<Guid, IFacility> facilities = _facilityManager.GetFacilities();
            GD.Print($"Facility Manager Initialized: {facilities.Count} facilities");

            CreateGrowingSpacePanels();
        }
    }

    public override void _Process(double delta)
    {
        _fpsLabel.Text = $"FPS: {Engine.GetFramesPerSecond()}";
    }

    private void OnResourceChanged(IResourceChangeEventArgs args)
    {
        _moneyLabel.Text = "$" + ResourceUtils.FormatAmount(_resourceManager.GetResource(ResourceType.Money));
    }

    private void OnGrowingSpacePurchaseButtonPressed()
    {
        if (_resourceManager.TrySpendResource(ResourceType.Money, 1000, "purchase growing spot"))
        {
            Panel growingSpacePanel = CreateGrowingSpacePanel();
            _growingSpacesContainer.AddChild(growingSpacePanel);
            AudioStreamPlayer2D audioPlayer = growingSpacePanel.GetNode<AudioStreamPlayer2D>("%PurchaseAudio");
            audioPlayer.Play();
            _growingSpacesContainer.MoveChild(growingSpacePanel, _growingSpacesContainer.GetChildCount() - 2);

            if (_facilityManager.SelectedFacility.GrowingSpaces.Count >= 21 && _growingSpacePurchasePanel != null)
            {
                _growingSpacePurchasePanel.Visible = false;
                _growingSpacePurchasePanel.QueueFree();
                _growingSpacePurchasePanel = null;
            }
        }
    }

    private void OnNewPlantButtonPressed(Guid guid)
    {
        IGrowingSpace growingSpace = _facilityManager.SelectedFacility.GetGrowingSpace(guid);

        if (growingSpace == null)
        {
            return;
        }
        
        HempAdventures.Scripts.Strains.Data.StrainData newStrain = _gameManager.StrainDatabase.GetRandomStrain();
        growingSpace.Plant(newStrain.Name);
    }

    private void CreateGrowingSpacePanels()
    {
        CreateGrowingSpacePurchasePanel();
        if (_facilityManager.SelectedFacility == null)
        {
            return;
        }

        for (int i = 0; i < _facilityManager.SelectedFacility.GrowingSpaces.Count; i++)
        {
            Panel growingSpacePanel = CreateGrowingSpacePanel();
            _growingSpacesContainer.AddChild(growingSpacePanel);
        }
    }

    private Panel CreateGrowingSpacePanel()
    {
        Panel growingSpacePanel = (Panel)GrowingSpacePanelScene.Instantiate();
        IGrowingSpace growingSpace = new GrowingSpace("", growingSpacePanel);
        _facilityManager.SelectedFacility.AddGrowingSpace(growingSpace);

        Button newPlantButton = growingSpacePanel.GetNode<Button>("%NewPlantButton");
        newPlantButton.Pressed += () => OnNewPlantButtonPressed(growingSpace.Guid);

        return growingSpacePanel;
    }

    private void CreateGrowingSpacePurchasePanel()
    {
        Panel growingSpacePurchasePanel = (Panel)GrowingSpacePurchasePanel.Instantiate();
        Button purchaseButton = growingSpacePurchasePanel.GetNode<Button>("%PurchaseButton");

        purchaseButton.Pressed += OnGrowingSpacePurchaseButtonPressed;

        _growingSpacesContainer.AddChild(growingSpacePurchasePanel);
        _growingSpacePurchasePanel = growingSpacePurchasePanel;
    }
}
