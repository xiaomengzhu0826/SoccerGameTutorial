using Godot;
using System;

public partial class PlayerState : Node
{
    [Signal] public delegate void OnStateTransitionRequestEventHandler(Player.State newState);

    protected Player _player = null;
    protected AnimationPlayer _animationPlayer = null;

    public void Setup(Player contextPlayer, AnimationPlayer contextAnimationPlayer)
    {
        _player = contextPlayer;
        _animationPlayer = contextAnimationPlayer;
    }


}
