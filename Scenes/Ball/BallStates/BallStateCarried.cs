using Godot;
using System;

public partial class BallStateCarried : BallState
{
    public override void _EnterTree()
    {
        if (_carrier == null)
        {
            throw new InvalidOperationException("Player is null");
        }
    }

    public override void _Process(double delta)
    {
        _ball.Position = _carrier.Position;
    }
}
