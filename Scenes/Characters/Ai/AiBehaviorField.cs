using Godot;
using System;

public partial class AiBehaviorField : AiBehavior
{
    private static readonly float SPREAD_ASSIST_FACTOR = 0.8f;
    private static readonly int SHOT_DISTANCE = 150;
    private static readonly float SHOT_PROBABILITY = 0.3f;
    private static readonly int TACKLE_DISTANCE = 15;
    private static readonly float TACKLE_PROBABILITY = 0.3f;
    private static readonly float PASS_PROBABILITY = 0.05f;

    protected override void PerformAiMovement()
    {
        var totalSteeringForce = Vector2.Zero;
        if (_player.HasBall())
        {
            totalSteeringForce += GetCarrierSteeringForce();
        }
        else 
        {
            totalSteeringForce += GetOndutySteeringForce();
            if (IsBallCarriedByTeammate())
            {
                totalSteeringForce += GetAssistFormationSteeringForce();
            }
        }

        totalSteeringForce = totalSteeringForce.LimitLength(1.0f);
        _player.Velocity = totalSteeringForce * _player._speed;
    }

    protected override void PerformAiDecisions()
    {
        if (IsBallPossessedByOppenent() && _player.Position.DistanceTo(_ball.Position) < TACKLE_DISTANCE && GD.Randf() < TACKLE_PROBABILITY)
        {
            _player.SwitchState(Player.State.TACKLING, null);
        }
        if (_ball._carrier == _player)
        {
            var target = _player._targetGoal.GetCenterTargetPosition();
            if (_player.Position.DistanceTo(target) < SHOT_DISTANCE && GD.RandRange(0, 1) < SHOT_PROBABILITY)
            {
                FaceTowardTargetGoal();
                var shotDirection = _player.Position.DirectionTo(_player._targetGoal.GetRandomTargetPosition());
                var data = PlayerStateData.Build().SetShotPower(_player._power).SetShotDirection(shotDirection);
                _player.SwitchState(Player.State.SHOOTING, data);
            }
            else if (HasOppenentsNearby() && GD.Randf() < PASS_PROBABILITY)
            {
                _player.SwitchState(Player.State.PASSING, null);
            }
        }
    }

    public Vector2 GetOndutySteeringForce()
    {
        return _player._weightOnDutySteering * _player.Position.DirectionTo(_ball.Position);
    }

    public Vector2 GetCarrierSteeringForce()
    {
        var target = _player._targetGoal.GetCenterTargetPosition();
        var direction = _player.Position.DirectionTo(target);
        var weight = GetBicircularWeight(_player.Position, target, 100, 0, 150, 1);
        return weight * direction;
    }

    public Vector2 GetAssistFormationSteeringForce()
    {
        var spawnDifference = _ball._carrier._spawnPosition - _player._spawnPosition;
        var assistDestination = _ball._carrier.Position - spawnDifference * SPREAD_ASSIST_FACTOR;
        var direction = _player.Position.DirectionTo(assistDestination);
        var weight = GetBicircularWeight(_player.Position, assistDestination, 30, 0.2f, 60, 1);
        return weight * direction;
    }
}
