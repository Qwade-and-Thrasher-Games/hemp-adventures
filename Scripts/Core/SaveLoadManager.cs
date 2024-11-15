using System;
using System.Collections.Generic;
using System.Text.Json;
using Godot;

namespace HempAdventures.Scripts.Core;

public partial class SaveLoadManager : Node
{

    public event Action OnSaveCompleted;
    public event Action OnLoadCompleted;

    public class GameSaveData
    {
        public Dictionary<Guid, IFacility> Facilities { get; set; }
        public Dictionary<ResourceType, decimal> Resources { get; set; }
        public DateTime LastSaveTime { get; set; }
        public DateTime LastOfflineTime { get; set; }


        public GameSaveData()
        {
            Facilities = new Dictionary<Guid, IFacility>();
            Resources = new Dictionary<ResourceType, decimal>();
        }
    }

    private GameSaveData _currentSaveData;

    public override void _Ready()
    {
        DirAccess.MakeDirRecursiveAbsolute(Constants.Game.Save.SAVE_DIR);
        _currentSaveData = new GameSaveData();
    }

    public bool HasSaveFile()
    {
        return Godot.FileAccess.FileExists(Constants.Game.Save.SAVE_DIR + Constants.Game.Save.SAVE_FILE_NAME);
    }

    public void SaveGame()
    {
        try
        {
            _currentSaveData.LastSaveTime = DateTime.Now;
            _currentSaveData.LastOfflineTime = DateTime.Now;

            string jsonString = JsonSerializer.Serialize(_currentSaveData, new JsonSerializerOptions
            {
                WriteIndented = true
            });

            string savePath = Constants.Game.Save.SAVE_DIR + Constants.Game.Save.SAVE_FILE_NAME;

            if (Godot.FileAccess.FileExists(savePath))
            {
                Godot.FileAccess backup = Godot.FileAccess.Open(savePath, Godot.FileAccess.ModeFlags.Read);
                string backupContent = backup.GetAsText();
                backup.Close();

                Godot.FileAccess backupFile = Godot.FileAccess.Open(savePath + Constants.Game.Save.BACKUP_EXT, Godot.FileAccess.ModeFlags.Write);
                backupFile.StoreString(backupContent);
                backupFile.Close();
            }

            Godot.FileAccess file = Godot.FileAccess.Open(savePath, Godot.FileAccess.ModeFlags.Write);
            file.StoreString(jsonString);
            file.Close();

            OnSaveCompleted?.Invoke();
            GD.Print("Game Saved!");
        }
        catch (Exception e)
        {
            GD.PrintErr($"Error saving game: {e.Message}");
        }
    }

    public void LoadGame()
    {
        try
        {
            string savePath = Constants.Game.Save.SAVE_DIR + Constants.Game.Save.SAVE_FILE_NAME;
            if (!Godot.FileAccess.FileExists(savePath))
            {
                GD.PrintErr("No save file found!");
                return;
            }
            Godot.FileAccess file = Godot.FileAccess.Open(savePath, Godot.FileAccess.ModeFlags.Read);
            string jsonString = file.GetAsText();
            file.Close();

            _currentSaveData = JsonSerializer.Deserialize<GameSaveData>(jsonString);

            OnLoadCompleted?.Invoke();
            GD.Print("Game Loaded!");
        }
        catch (Exception e)
        {
            GD.PrintErr($"Error loading game: {e.Message}");
            AttemptLoadBackup();
        }
    }

    private void AttemptLoadBackup()
    {
        try
        {
            string savePath = Constants.Game.Save.SAVE_DIR + Constants.Game.Save.SAVE_FILE_NAME + Constants.Game.Save.BACKUP_EXT;
            if (!Godot.FileAccess.FileExists(savePath))
            {
                GD.PrintErr("No backup save file found!");
                return;
            }
            Godot.FileAccess file = Godot.FileAccess.Open(savePath, Godot.FileAccess.ModeFlags.Read);
            string jsonString = file.GetAsText();
            file.Close();
            _currentSaveData = JsonSerializer.Deserialize<GameSaveData>(jsonString);
            OnLoadCompleted?.Invoke();
            GD.Print("Backup Loaded!");
        }
        catch (Exception e)
        {
            GD.PrintErr($"Error loading backup: {e.Message}");
            _currentSaveData = new GameSaveData();
            AddResource(ResourceType.Money, Constants.Game.STARTING_MONEY);

            IFacility newFacility = new Facility("Testing Facility", FacilityType.Small);
            AddFacility(newFacility);
        }
    }

    public void AddFacility(IFacility facility)
    {
        _currentSaveData.Facilities[facility.Id] = facility;
    }

    public void RemoveFacility(Guid id)
    {
        _currentSaveData.Facilities.Remove(id);
    }

    public IFacility GetFacility(Guid id)
    {
        return _currentSaveData.Facilities.GetValueOrDefault(id);
    }

    public Dictionary<Guid, IFacility> GetFacilities()
    {
        return _currentSaveData.Facilities;
    }

    public void AddResource(ResourceType type, decimal amount)
    {
        if (_currentSaveData.Resources.ContainsKey(type))
        {
            _currentSaveData.Resources[type] += amount;
        }
        else
        {
            _currentSaveData.Resources[type] = amount;
        }
    }

    public decimal GetResource(ResourceType type)
    {
        return _currentSaveData.Resources.GetValueOrDefault(type, 0);
    }

    public Dictionary<ResourceType, decimal> GetResources()
    {
        return _currentSaveData.Resources;
    }

    public void UpdateResource(ResourceType type, decimal amount)
    {
        decimal currentAmount = GetResource(type);

        if (currentAmount + amount < 0)
        {
            GD.PrintErr($"Insufficient [{type}] to spend {amount}");
            return;
        }

        _currentSaveData.Resources[type] = currentAmount + amount;
        GD.Print($"Resource Updated: [{type}] {_currentSaveData.Resources[type]}");
    }
}