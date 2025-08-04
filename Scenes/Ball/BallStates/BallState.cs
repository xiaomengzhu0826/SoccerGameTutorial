using Godot;
using System;

public partial class BallState : Node
{
    [Signal] public delegate void OnStateTransitionRequestEventHandler(Ball.State newState);

    protected Ball _ball;
    protected Player _carrier;
    protected Area2D _playerDetectionArea;

    public void Setup(Ball contextBall, Area2D contextPlayerDetectionArea, Player contextCarrier)
    {
        _ball = contextBall;
        _playerDetectionArea = contextPlayerDetectionArea;
        _carrier = contextCarrier;
    }
}
