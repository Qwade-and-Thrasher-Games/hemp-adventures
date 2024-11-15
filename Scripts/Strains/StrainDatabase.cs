using Godot;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public partial class StrainDatabase : Node
{
	private List<HempAdventures.Scripts.Strains.Data.StrainData> _strains = new();

	private static StrainDatabase _strainDatabase;
    public static StrainDatabase Instance => _strainDatabase;

    public override void _Ready()
	{
        if (_strainDatabase == null)
        {
            _strainDatabase = this;
        }
        else
        {
            QueueFree();
        }

        LoadStrains();
    }

    private void LoadStrains()
    {
        string filePath = "res://Assets/Data/strain_names.csv";

        using var file = FileAccess.Open(filePath, FileAccess.ModeFlags.Read);
        if (file == null)
        {
            GD.PrintErr($"Failed to load strains file from: {filePath}");
            return;
        }

        file.GetLine();

        while(!file.EofReached())
        {
            string line = file.GetLine().StripEdges();
            
            if (!string.IsNullOrEmpty(line))
            {
                _strains.Add(new HempAdventures.Scripts.Strains.Data.StrainData(line));
            }
        }

        GD.Print($"Loaded {_strains.Count} strains!");
    }

    public HempAdventures.Scripts.Strains.Data.StrainData GetStrainByName(string name)
    {
        return _strains.FirstOrDefault(s => s.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
    }

    public List<HempAdventures.Scripts.Strains.Data.StrainData> GetAllStrains()
    {
        return _strains;
    }

    public List<HempAdventures.Scripts.Strains.Data.StrainData> SearchStrains(string search)
    {
        return _strains
            .Where(s => s.Name.Contains(search, StringComparison.OrdinalIgnoreCase))
            .ToList();
    }

    public int GetStrainsCount()
    {
        return _strains.Count;
    }

    public List<HempAdventures.Scripts.Strains.Data.StrainData> GetRandomStrains(int count)
    {
        var random = new Random();
        return _strains
            .OrderBy(x => random.Next())
            .Take(Math.Min(count, _strains.Count))
            .ToList();
    }

    public HempAdventures.Scripts.Strains.Data.StrainData GetRandomStrain()
    {
        return GetRandomStrains(1).FirstOrDefault();
    }
}
