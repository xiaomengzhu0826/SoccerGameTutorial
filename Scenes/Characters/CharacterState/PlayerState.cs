using Godot;
using System;

public partial class PlayerState : Node
{
    [Signal] public delegate void OnStateTransitionRequestEventHandler(Player.State newState,PlayerStateData? stateData);

    protected Player _player;
    protected Ball _ball;
    protected AnimationPlayer _animationPlayer;
    protected PlayerStateData _playerStateData;

    public void Setup(Player contextPlayer, PlayerStateData contextPlayerStateData, AnimationPlayer contextAnimationPlayer, Ball contextBall)
    {
        _player = contextPlayer;
        _animationPlayer = contextAnimationPlayer;
        _playerStateData = contextPlayerStateData;
        _ball = contextBall;
    }

    // public void TransitionState(Player.State newState,PlayerStateData stateData)
    // {
    //     EmitSignal(SignalName.OnStateTransitionRequest, (int)newState, stateData);
    // }

    public virtual void OnAnimationCompelete()
    {

    }


}
