using Godot;
using System;
using System.Collections.Generic;

public partial class GameStateKickOff : GameState
{
    private List<Player.ControlScheme> _validControlSchemes = new();

    public override void _EnterTree()
    {
        string countryStarting;
        if (_gameStateData != null)
        {
            countryStarting = _gameStateData._CountryScoredOn;
        }
        else
        {
            countryStarting = _gameManager._Countries[0];
        }

        if (countryStarting == _gameManager._PlayerSetup[0])
        {
            _validControlSchemes.Add(Player.ControlScheme.P1);
        }
        if (countryStarting == _gameManager._PlayerSetup[1])
        {
            _validControlSchemes.Add(Player.ControlScheme.P2);
        }
        if (_validControlSchemes.Count == 0)
        {
            _validControlSchemes.Add(Player.ControlScheme.P1);
        }


    }

    public override void _Process(double delta)
    {
        foreach (Player.ControlScheme controlScheme in _validControlSchemes)
        {
            if (KeyUtils.IsActionJustPressed(controlScheme, KeyUtils.Action.PASS))
            {
                SignalManager.EmitOnKickoffStarted();
                SoundManager.Instance.Play(SoundManager.Sound.WHISTLE);
                TransitionState(GameManager.State.IN_PLAY);
            }
        }
    }

}
