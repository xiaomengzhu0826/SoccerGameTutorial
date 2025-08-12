using Godot;
using System;

public partial class AiBehavior : Node
{
    private static readonly int DURATION_AI_TICK_FREQUENCY = 200;
    private static readonly float SPREAD_ASSIST_FACTOR = 0.8f;
    private static readonly int SHOT_DISTANCE = 150;
    private static readonly float SHOT_PROBABILITY = 0.3f;

    public Ball _ball;
    public Player _player;
    private float _timeSinceLastAiTick = Time.GetTicksMsec();

    public void Setup(Player contextPlayer, Ball contextBall)
    {
        _player = contextPlayer;
        _ball = contextBall;

        _timeSinceLastAiTick = Time.GetTicksMsec() + (float)GD.RandRange(0, DURATION_AI_TICK_FREQUENCY);
    }

    public void ProcessAi()
    {
        if (Time.GetTicksMsec() - _timeSinceLastAiTick > DURATION_AI_TICK_FREQUENCY)
        {
            _timeSinceLastAiTick = Time.GetTicksMsec();
            PerformAiMovement();
            PerformAiDecisions();
        }

    }

    private void PerformAiMovement()
    {
        var totalSteeringForce = Vector2.Zero;
        if (_player.HasBall())
        {
            totalSteeringForce += GetCarrierSteeringForce();
        }
        else if (_player._role != Player.Role.GOALIE)
        {
            totalSteeringForce += GetOndutySteeringForce();
            if (IsBallCarriedByTeammate())
            {
                totalSteeringForce += GetAssistFormationSteering();
            }
        }

        totalSteeringForce = totalSteeringForce.LimitLength(1.0f);
        _player.Velocity = totalSteeringForce * _player._speed;
    }

    private void PerformAiDecisions()
    {

        if (_ball._carrier==_player)
        {
            var target = _player._targetGoal.GetCenterTargetPosition();
            if (_player.Position.DistanceTo(target) < SHOT_DISTANCE && GD.RandRange(0, 1) < SHOT_PROBABILITY)
            {
                FaceTowardTargetGoal();
                var shotDirection = _player.Position.DirectionTo(_player._targetGoal.GetRandomTargetPosition());
                var data = PlayerStateData.Build().SetShotPower(_player._power).SetShotDirection(shotDirection);
                _player.SwitchState(Player.State.SHOOTING, data);
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

    public Vector2 GetAssistFormationSteering()
    {
        var spawnDifference = _ball._carrier._spawnPosition - _player._spawnPosition;
        var assistDestination = _ball._carrier.Position - spawnDifference * SPREAD_ASSIST_FACTOR;
        var direction = _player.Position.DirectionTo(assistDestination);
        var weight = GetBicircularWeight(_player.Position, assistDestination, 30, 0.2f, 60, 1);
        return weight * direction;
    }

    public float GetBicircularWeight(Vector2 position, Vector2 centerTarget, float innerCircleRadius, float innerCircleWeight, float outerCircleRadius, float outerCircleWeight)
    {
        var distanceToCenter = position.DistanceTo(centerTarget);
        if (distanceToCenter > outerCircleRadius)
        {
            return outerCircleWeight;
        }
        else if (distanceToCenter < innerCircleRadius)
        {
            return innerCircleWeight;
        }
        else
        {
            var distanceToInnerRadius = distanceToCenter - innerCircleRadius;
            var closeRangeDistance = outerCircleRadius - innerCircleRadius;
            return Mathf.Lerp(innerCircleWeight, outerCircleWeight, distanceToInnerRadius / closeRangeDistance);
        }
    }

    private void FaceTowardTargetGoal()
    {
        if (!_player.IsFacingTargetGoal())
        {
            _player._heading *= -1;
        }
    }

    public bool IsBallCarriedByTeammate()
    {
        return _ball._carrier != null && _ball._carrier != _player && _ball._carrier._country == _player._country;
    }
}
