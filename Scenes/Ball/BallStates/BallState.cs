using Godot;
using System;

public partial class BallState : Node
{
    [Signal] public delegate void OnStateTransitionRequestEventHandler(Ball.State newState);

    protected Ball _ball;
    protected Player _carrier;
    protected Area2D _playerDetectionArea;
    protected AnimationPlayer _animationPlayer;
    protected Sprite2D _ballSprite;

    public void Setup(Ball contextBall, Area2D contextPlayerDetectionArea, Player contextCarrier, AnimationPlayer contextAnimationPlayer, Sprite2D contextBallSprite)
    {
        _ball = contextBall;
        _playerDetectionArea = contextPlayerDetectionArea;
        _carrier = contextCarrier;
        _animationPlayer = contextAnimationPlayer;
        _ballSprite = contextBallSprite;
    }
}
