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
        SetBallAnimationFromVelocity();
        _ballSprite.Scale = new Vector2(1, SHOT_SPRITE_SCALE);
        _ball._height = SHOT_HEIGHT;
        _timeSinceShot = Time.GetTicksMsec();
        _shotParticles.Emitting = true;
        SignalManager.EmitOnImpactReceived(_ball.Position, true);
    }

    public override void _ExitTree()
    {
        _ballSprite.Scale = new Vector2(1, 1);
        _shotParticles.Emitting = false;
    }

    public override void _Process(double delta)
    {
        if (Time.GetTicksMsec() - _timeSinceShot > DURATION_SHOT)
        {
            EmitSignal(BallState.SignalName.OnStateTransitionRequest,(int)Ball.State.FREEFORM,BallStateData.Build().SetLockDuration(200));
        }
        else
        {
            MoveAndBounce((float)delta);
        } 
    }
}
