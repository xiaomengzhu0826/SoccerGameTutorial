using Godot;
using System;

public partial class PlayerStateReseting : PlayerState
{
    private bool _hasArrived;

    public override void _Process(double delta)
    {
        if (!_hasArrived)
        {
            var direction = _player.Position.DirectionTo(_playerStateData._ResetPosition);
            if (_player.Position.DistanceSquaredTo(_playerStateData._ResetPosition) < 2)
            {
                _hasArrived = true;
                _player.Velocity = Vector2.Zero;
                _player.FaceTowardTargetGoal();
            }
            else
            {
                _player.Velocity = direction * _player._Speed;
            }
            _player.SetMovementAnimation();
            _player.SetHeading();
        }
    }

    public override bool IsReadyForKickoff()
    {
        return _hasArrived;
    }
}
