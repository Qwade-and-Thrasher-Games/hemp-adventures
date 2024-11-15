using Godot;
using System;

public partial class StartScreenUI : Node2D
{
	private Button startButton;
    private HempAdventures.Scripts.Core.GameManager _gameManager;
    public override void _Ready()
	{
        _gameManager = GetTree().Root.GetNode<HempAdventures.Scripts.Core.GameManager>("GameManager");

        if (_gameManager == null)
        {
            GetTree().Quit();
        }

        Button continueButton = GetNode<Button>("%ContinueButton");
        Button newGameButton = GetNode<Button>("%NewGameButton");

        continueButton.Pressed += HandleContinueButtonPressed;
        newGameButton.Pressed += HandleNewGameButtonPressed;

        if (_gameManager.SaveLoadManager.HasSaveFile())
        {
            continueButton.Disabled = false;
        }
        else
        {
            continueButton.Disabled = true;
        }
    }

    private void HandleContinueButtonPressed()
	{
        _gameManager.HandleLoad();
    }

    private void HandleNewGameButtonPressed()
    {
        _gameManager.CreateNewGame();
    }

}
