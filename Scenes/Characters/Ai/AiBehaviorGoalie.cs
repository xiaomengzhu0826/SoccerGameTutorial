using Godot;
using System;

public partial class AiBehaviorGoalie : AiBehavior
{
    private static readonly float PROXIMITY_CONCERN = 10.0f;

    protected override void PerformAiMovement()
    {
        var totalSteeringForce = GetGoalieSteeringForce();
        totalSteeringForce = totalSteeringForce.LimitLength(1.0f);
        _player.Velocity = totalSteeringForce * _player._speed;
    }

    protected override void PerformAiDecisions()
    {
        if (_ball.IsHeadedForScoringArea(_player._ownGoal.GetScoringArea()))
        {
            _player.SwitchState(Player.State.DIVING, null);
        }
    }

    private Vector2 GetGoalieSteeringForce()
    {
        var top = _player._ownGoal.GetTopTargetPosition();
        var bottom = _player._ownGoal.GetBottomTargetPosition();
        var center = _player._spawnPosition;
        var targetY = Mathf.Clamp(_ball.Position.Y, top.Y, bottom.Y);
        var destination =new Vector2(center.X, targetY);
        var direction = _player.Position.DirectionTo(destination);
        var distanceToDestination = _player.Position.DistanceTo(destination);
        var weight = Mathf.Clamp(distanceToDestination / PROXIMITY_CONCERN, 0, 1);

        return weight*direction;
    }
}
