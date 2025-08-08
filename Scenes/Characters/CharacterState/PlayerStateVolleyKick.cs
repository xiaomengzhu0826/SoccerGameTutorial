using Godot;
using System;

public partial class PlayerStateVolleyKick : PlayerState
{
    private static readonly float BONUS_POWER = 1.5f;
    private static readonly float BALL_HEIGHT_MIN = 1f;
    private static readonly float BALL_HEIGHT_MAX = 40f;

    public override void _EnterTree()
    {
        _animationPlayer.Play("volley_kick");
        _ballDetectionArea.BodyEntered += OnBallEntered;
    }

    private void OnBallEntered(Node2D body)
    {
        Ball contactBall = (Ball)body;
        if (contactBall.CanAirConnect(BALL_HEIGHT_MIN,BALL_HEIGHT_MAX))
        {
            var destination = _targetGoal.GetRandomTargetPosition();
            var direction = _ball.Position.DirectionTo(destination);
            contactBall.Shoot(direction * _player._power * BONUS_POWER);
        }
    }

    public override void OnAnimationCompelete()
    {
       EmitSignal(PlayerState.SignalName.OnStateTransitionRequest, (int)Player.State.RECOVERING,(PlayerStateData)null);
    }


}
