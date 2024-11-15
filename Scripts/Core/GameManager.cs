using System;
using System.Collections.Generic;
using Godot;
using HempAdventures.Scripts.Inventory;
using HempAdventures.Scripts.Resources.Interfaces;

namespace HempAdventures.Scripts.Core;

public partial class GameManager : Node
{
    private static GameManager _instance;
    public static GameManager Instance => _instance;

    private IResourceManager _resourceManager;
    public IResourceManager ResourceManager => _resourceManager;

    private FacilityManager _facilityManager;
    public FacilityManager FacilityManager => _facilityManager;

    private StrainDatabase _strainDatabase;
    public StrainDatabase StrainDatabase => _strainDatabase;

    private SaveLoadManager _saveLoadManager;
    public SaveLoadManager SaveLoadManager => _saveLoadManager;
    
    private InventoryManager _inventoryManager;
    public InventoryManager InventoryManager => _inventoryManager;

    public override void _Ready()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else
        {
            QueueFree();
        }

        InitializeManagers();

        _facilityManager.OnFacilityAdded += OnFacilityAdded;
        _facilityManager.OnFacilityRemoved += OnFacilityRemoved;

        _resourceManager.OnResourceChanged += OnResourceChanged;


        GD.Print("GameManager Ready!");
        GD.Print("Game Version: " + Constants.Game.GAME_VERSION);
        GD.Print("Strain Count: " + _strainDatabase.GetStrainsCount());
    }

    private void OnFacilityAdded(IFacility facility)
    {
        _saveLoadManager.AddFacility(facility);
    }

    private void OnFacilityRemoved(IFacility facility)
    {
        _saveLoadManager.RemoveFacility(facility.Id);
    }

    private void OnResourceChanged(IResourceChangeEventArgs args)
    {
        GD.Print($"Resource Changed: {args.Type} {args.Amount} {args.Source}");
        _saveLoadManager.UpdateResource(args.Type, args.Amount);
    }

    public override void _ExitTree()
    {
        _saveLoadManager.SaveGame();
    }

    public void CreateNewGame()
    {
        _resourceManager.AddResource(ResourceType.Money, Constants.Game.STARTING_MONEY, "Starting Funds");

        IFacility newFacility = new Facility("Testing Facility", FacilityType.Small);
        _facilityManager.AddFacility(newFacility);
        _facilityManager.SelectFacility(newFacility.Id);


        GetTree().ChangeSceneToFile("res://Scenes/Main.tscn");
    }

    public void HandleLoad()
    {
        _saveLoadManager.LoadGame();

        Dictionary<ResourceType, decimal> resources = _saveLoadManager.GetResources();
        foreach (KeyValuePair<ResourceType, decimal> resource in resources)
        {
            _resourceManager.AddResource(resource.Key, resource.Value, "Loaded Game");
        }

        Dictionary<Guid, IFacility> facilities = _saveLoadManager.GetFacilities();
        foreach (KeyValuePair<Guid, IFacility> facility in facilities)
        {
            _facilityManager.AddFacility(facility.Value);
        }

        _facilityManager.SelectFacility(facilities.Keys.GetEnumerator().Current);

        GetTree().ChangeSceneToFile("res://Scenes/Main.tscn");
    }

    private void InitializeManagers()
    {
        _resourceManager = GetTree().Root.GetNode<IResourceManager>("ResourceManager");
        _facilityManager = GetTree().Root.GetNode<FacilityManager>("FacilityManager");
        _strainDatabase = GetTree().Root.GetNode<StrainDatabase>("StrainDatabase");
        _saveLoadManager = GetTree().Root.GetNode<SaveLoadManager>("SaveLoadManager");
        _inventoryManager = GetTree().Root.GetNode<InventoryManager>("InventoryManager");

        GD.Print("Managers Initialized!");
    }
}