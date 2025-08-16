using Godot;
using System;

public partial class PlayerStateCelebrate : PlayerState
{
    private static readonly float CELEBRATE_HEIGHT = 1.0f;
    private static readonly float AIR_FRICTION = 35.0f;

    public override void _EnterTree()
    {
        Celebrate();
    }

    public override void _Process(double delta)
    {
        if (_player._height == 0)
        {
            Celebrate();
        }
        _player.Velocity = _player.Velocity.MoveToward(Vector2.Zero, (float)delta * AIR_FRICTION);
    }

    private void Celebrate()
    {
        _animationPlayer.Play("celebrate");
        _player._height = 0.1f;
        _player._heightVelocity = CELEBRATE_HEIGHT;
    }
}
