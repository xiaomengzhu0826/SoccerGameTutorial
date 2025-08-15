using Godot;
using System;

public partial class AiBehavior : Node
{
    private static readonly int DURATION_AI_TICK_FREQUENCY = 200;


    public Ball _ball;
    public Player _player;
    public Area2D _opponentDetectionArea;
    public Area2D _teammateDetectionArea;
    private float _timeSinceLastAiTick = Time.GetTicksMsec();

    public void Setup(Player contextPlayer, Ball contextBall, Area2D contextOpponentDetectionArea,Area2D contextTeammateDetectionArea)
    {
        _player = contextPlayer;
        _ball = contextBall;
        _opponentDetectionArea = contextOpponentDetectionArea;
        _teammateDetectionArea = contextTeammateDetectionArea;
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

    protected virtual void PerformAiMovement()
    {

    }

    protected virtual void PerformAiDecisions()
    {
        
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

    protected void FaceTowardTargetGoal()
    {
        if (!_player.IsFacingTargetGoal())
        {
            _player._heading *= -1;
        }
    }

    public bool IsBallPossessedByOppenent()
    {
        return _ball._carrier != null && _ball._carrier._country != _player._country;
    }

    public bool IsBallCarriedByTeammate()
    {
        return _ball._carrier != null && _ball._carrier != _player && _ball._carrier._country == _player._country;
    }

    public bool HasOppenentsNearby()
    {
        var players = _opponentDetectionArea.GetOverlappingBodies();
        var opponent = players.FindOpponents(_player);
        if (opponent != null)
        {
            return true;
        }
        return false;

    }
}
