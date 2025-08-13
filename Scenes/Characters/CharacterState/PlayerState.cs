using Godot;
using System;

public partial class PlayerState : Node
{
    [Signal] public delegate void OnStateTransitionRequestEventHandler(Player.State newState, PlayerStateData stateData);

    protected Player _player;
    protected Ball _ball;
    protected AnimationPlayer _animationPlayer;
    protected PlayerStateData _playerStateData;
    protected Area2D _teammateDetectionArea;
    protected Area2D _ballDetectionArea;
    protected Goal _ownGoal;
    protected Goal _targetGoal;
    protected AiBehavior _aiBehavior;
    protected Area2D _tackleDamageEmitterArea;

    public void Setup(Player contextPlayer,
                        PlayerStateData contextPlayerStateData,
                        AnimationPlayer contextAnimationPlayer,
                        Ball contextBall,
                        Area2D contextTeammateDetectionArea,
                        Area2D contextBallDetectionArea,
                        Goal contextOwnGoal,
                        Goal contextTargetGoal,
                        Area2D contextTackleDamageEmitterArea,
                        AiBehavior contextAiBehavior)
    {
        _player = contextPlayer;
        _animationPlayer = contextAnimationPlayer;
        _playerStateData = contextPlayerStateData;
        _ball = contextBall;
        _teammateDetectionArea = contextTeammateDetectionArea;
        _ballDetectionArea = contextBallDetectionArea;
        _ownGoal = contextOwnGoal;
        _targetGoal = contextTargetGoal;
        _tackleDamageEmitterArea = contextTackleDamageEmitterArea;
        _aiBehavior = contextAiBehavior;
    }

    // public void TransitionState(Player.State newState,PlayerStateData stateData)
    // {
    //     EmitSignal(SignalName.OnStateTransitionRequest, (int)newState, stateData);
    // }

    public virtual void OnAnimationCompelete()
    {

    }


}
