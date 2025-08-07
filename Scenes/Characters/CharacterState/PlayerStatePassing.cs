using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class PlayerStatePassing : PlayerState
{
    public override void _EnterTree()
    {
        _animationPlayer.Play("kick");
        _player.Velocity = Vector2.Zero;
    }

    public override void OnAnimationCompelete()
    {
        Player passTarget = FindTeammateInView();
        EmitSignal(PlayerState.SignalName.OnStateTransitionRequest, (int)Player.State.MOVING, (PlayerStateData)null);
    }

    private Player FindTeammateInView()
    {
        var playersInView = _teammateDetectionArea.GetOverlappingBodies();
        List<Node2D> teammatesInView = new();
        foreach (var item in playersInView)
        {
            if (item != _player)
            {
                teammatesInView.Add(item);
            }
        }
        teammatesInView.SortByDistance(_player.GlobalPosition, p => p.GlobalPosition);
        
        if (teammatesInView.Count > 0)
        {
            //GD.Print(teammatesInView[0]);
            return (Player)teammatesInView[0];
        }
        else
        {
            //GD.Print("null");
            return null;
        }
        
    }

}
 