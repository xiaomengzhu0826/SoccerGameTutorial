using Godot;
using System;
using System.Collections.Generic;

public partial class BallStateFactory : Node
{
    public Dictionary<Ball.State,  Func<BallState>> _states;

    public BallStateFactory()
    {
        _states = new()
        {
            {Ball.State.CARRIED, () => new BallStateCarried() },
            {Ball.State.FREEFORM ,() =>new BallStateFreeform()},
            {Ball.State.SHOT ,() =>new BallStateShot()}
        };
    }

    public BallState GetFreshState(Ball.State state)
    {
        if (!_states.ContainsKey(state))
        {
            throw new System.Exception("State does not exist");
        }
        return _states[state]();
    }
}
