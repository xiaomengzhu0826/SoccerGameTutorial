using Godot;
using System;

public partial class BallStateCarried : BallState
{
    private readonly float DRIBBLE_FREQUENCY =10;
	private readonly float DRIBBLE_INTENSITY=3;

    private readonly Vector2 OFFSET_FROM_PLAYER = new Vector2(10, 4);
    private float _dribbleTime = 0.0f;

    public override void _EnterTree()
    {
        if (_carrier == null)
        {
            throw new InvalidOperationException("Player is null");
        }
    }

    public override void _Process(double delta)
    {
        var vx = 0.0f;
        _dribbleTime += (float)delta;
        if (_carrier.Velocity != Vector2.Zero)
        {
            if (_carrier.Velocity.X != 0)
            {
                vx = Mathf.Cos(_dribbleTime*DRIBBLE_FREQUENCY) * DRIBBLE_INTENSITY;
            }
            
            if (_carrier._heading.X > 0)
            {
                _animationPlayer.Play("roll");
                _animationPlayer.Advance(0);
            }
            else
            {
                _animationPlayer.PlayBackwards("roll");
                _animationPlayer.Advance(0);
            }
        }
        else
        {
            _animationPlayer.Play("idle");
        }
        ProcessGravity((float)delta);
        _ball.Position = _carrier.Position + new Vector2(vx+_carrier._heading.X * OFFSET_FROM_PLAYER.X, OFFSET_FROM_PLAYER.Y);
    }
}
