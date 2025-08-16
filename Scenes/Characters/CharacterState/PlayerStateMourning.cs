using Godot;
using System;

public partial class PlayerStateMourning : PlayerState
{
    public override void _EnterTree()
    {
        _animationPlayer.Play("mourn");
        _player.Velocity = Vector2.Zero;
    }
}
