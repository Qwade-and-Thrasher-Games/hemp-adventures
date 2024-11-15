using Godot;
using System;

public partial class GrowingSpacePanel : Panel
{
	private Timer _growingTimer;
	private Timer _uiUpdateTimer;
    private Label _progressLabel;
    private TextureRect _budOverlay;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
		_growingTimer = GetNode<Timer>("%GrowingTimer");
        _uiUpdateTimer = GetNode<Timer>("%UIUpdateTimer");
        _progressLabel = GetNode<Label>("%ProgressLabel");
        _budOverlay = GetNode<TextureRect>("%BudOverlay");

        _uiUpdateTimer.Timeout += OnUIUpdateTimer;

        ShaderMaterial shaderMaterial = (ShaderMaterial)_budOverlay.Material;
        ShaderMaterial uniqueMaterial = (ShaderMaterial)shaderMaterial.Duplicate();
        _budOverlay.Material = uniqueMaterial;

        float hue = GD.Randf();          // Random hue between 0 and 1
        float saturation = 1.0f;         // Full saturation
        float value = 1.0f;              // Full brightness
        Color vibrantColor = Color.FromHsv(hue, saturation, value);

        uniqueMaterial.SetShaderParameter("replacement_color", vibrantColor);

        uniqueMaterial.SetShaderParameter("hue_tolerance", 0.1f);      // Tighter hue tolerance
        uniqueMaterial.SetShaderParameter("saturation_tolerance", 0.2f);
        uniqueMaterial.SetShaderParameter("value_tolerance", 0.2f);
        uniqueMaterial.SetShaderParameter("intensity", 1.0f);          // Full intensity
        uniqueMaterial.SetShaderParameter("smoothness", 0.1f);         // Less smoothing
    }

    private void OnUIUpdateTimer()
    {
        _progressLabel.Text = GetProgressPercentage();
    }

    private string GetProgressPercentage()
    {
        if (_growingTimer.IsStopped())
        {
            return "Progress: 0%";
        }

        return $"Progress: {Mathf.Round((1 - _growingTimer.TimeLeft / _growingTimer.WaitTime) * 100)}%";
    }
}
