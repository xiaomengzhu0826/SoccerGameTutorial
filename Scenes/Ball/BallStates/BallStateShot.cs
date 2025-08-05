using Godot;
using System;

public partial class BallStateShot : BallState
{
    private readonly float SHOT_SPRITE_SCALE = 0.8f;
    private readonly int SHOT_HEIGHT = 5;
    private readonly int DURATION_SHOT = 1000;

    private float _timeSinceShot=Time.GetTicksMsec();


    public override void _EnterTree()
    {
        if (_ball._velocity.X >= 0)
        {
            _animationPlayer.Play("roll");
            _animationPlayer.Advance(0);
        }
        else
        {
            _animationPlayer.PlayBackwards("roll");
            _animationPlayer.Advance(0);
        }
        _ballSprite.Scale = new Vector2(1, SHOT_SPRITE_SCALE);
        _ball._height = SHOT_HEIGHT;
        _timeSinceShot = Time.GetTicksMsec();
    }

    public override void _ExitTree()
    {
        _ballSprite.Scale = new Vector2(1, 1);
    }

    public override void _Process(double delta)
    {
        if (Time.GetTicksMsec() - _timeSinceShot > DURATION_SHOT)
        {
            EmitSignal(BallState.SignalName.OnStateTransitionRequest,(int)Ball.State.FREEFORM);
        }
        else
        {
             _ball.MoveAndCollide(_ball._velocity * (float)delta);
        }
       
    }
}
