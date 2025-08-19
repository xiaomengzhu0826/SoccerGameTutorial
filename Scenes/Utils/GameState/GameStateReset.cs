using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class GameStateReset : GameState
{
    private List<Player> _playersList = new();

    public override void _EnterTree()
    {
        SignalManager.EmitOnTeamReset();
        SignalManager.Instance.OnKickoffReady += OnKickoffReady;
    }

    public override void _ExitTree()
    {
        SignalManager.Instance.OnKickoffReady -= OnKickoffReady;
    }

    private void OnKickoffReady()
    {
        TransitionState(GameManager.State.KICKOFF,_gameStateData);
    }
}
