using Godot;
using System;
using System.Linq;

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
        else if (IsBallCarriedByTeammate())
        {
            totalSteeringForce += GetAssistFormationSteeringForce();
        }
        else
        {
            totalSteeringForce += GetOndutySteeringForce();
            if (totalSteeringForce.LengthSquared() < 1)
            {
                if (IsBallPossessedByOppenent())
                {
                    totalSteeringForce += GetSpawnSteeringForce();
                }
                else if (_ball._carrier == null)
                {
                    totalSteeringForce += GetBallProximitySteeringForce();
                    totalSteeringForce += GetDensityAroundBallSteeringForce();
                }

            }
        }

        totalSteeringForce = totalSteeringForce.LimitLength(1.0f);
        _player.Velocity = totalSteeringForce * _player._Speed;
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
                _player.FaceTowardTargetGoal();
                var shotDirection = _player.Position.DirectionTo(_player._targetGoal.GetRandomTargetPosition());
                var data = PlayerStateData.Build().SetShotPower(_player._Power).SetShotDirection(shotDirection);
                _player.SwitchState(Player.State.SHOOTING, data);
            }
            else if (GD.Randf() < PASS_PROBABILITY && HasOppenentsNearby() && HasTeammateInView())
            {
                _player.SwitchState(Player.State.PASSING, null);
            }
        }
    }

    public Vector2 GetOndutySteeringForce()
    {
        return _player._WeightOnDutySteering * _player.Position.DirectionTo(_ball.Position);
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
        var spawnDifference = _ball._carrier._SpawnPosition - _player._SpawnPosition;
        var assistDestination = _ball._carrier.Position - spawnDifference * SPREAD_ASSIST_FACTOR;
        var direction = _player.Position.DirectionTo(assistDestination);
        var weight = GetBicircularWeight(_player.Position, assistDestination, 30, 0.2f, 60, 1);
        return weight * direction;
    }

    public Vector2 GetBallProximitySteeringForce()
    {
        var weight = GetBicircularWeight(_player.Position, _ball.Position, 50f, 1f, 120f, 0);
        var direction = _player.Position.DirectionTo(_ball.Position);
        return weight * direction;
    }

    public Vector2 GetSpawnSteeringForce()
    {
        var weight = GetBicircularWeight(_player.Position, _player._SpawnPosition, 30, 0, 100, 1);
        var direction = _player.Position.DirectionTo(_player._SpawnPosition);
        return weight * direction;
    }

    public Vector2 GetDensityAroundBallSteeringForce()
    {
        var nbTeammatesNearBall = _ball.GetProximityTeammatesCount(_player._Country);
        if (nbTeammatesNearBall == 0)
        {
            return Vector2.Zero;
        }
        var weight = 1 - 1.0 / nbTeammatesNearBall;
        var direction = _ball.Position.DirectionTo(_player.Position);
        return (float)weight * direction;
    }

    public bool HasTeammateInView()
    {
        var playersInView = _teammateDetectionArea.GetOverlappingAreas();
        return playersInView.FindTeammates(_player).Count() != 0;
    }


}
