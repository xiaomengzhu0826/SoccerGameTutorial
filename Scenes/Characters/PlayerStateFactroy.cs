using Godot;
using System;
using System.Collections.Generic;

public class PlayerStateFactroy
{
    public Dictionary<Player.State,  Func<PlayerState>> _states;

    public PlayerStateFactroy()
    {
        _states = new()
        {
            {Player.State.MOVING, () => new PlayerStateMoving() },
            {Player.State.TACKLING ,() =>new PlayerStateTackling()},
            {Player.State.RECOVERING ,() =>new PlayerStateRecovering()},
            {Player.State.PREPPING_SHOT ,() =>new PlayerStatePreppingShot()},
            {Player.State.SHOOTING ,() =>new PlayerStateShooting()},
            {Player.State.PASSING ,() =>new PlayerStatePassing()},
            {Player.State.HEADER ,() =>new PlayerStateHeader()},
            {Player.State.BICYCLE_KICK ,() =>new PlayerStateBicycleKick()},
            {Player.State.VOLLEY_KICK ,() =>new PlayerStateVolleyKick()},
            {Player.State.CHEST_CONTROL ,() =>new PlayerStateChestControl()},
            {Player.State.HURT ,() =>new PlayerStateHurt()},
            {Player.State.DIVING ,() =>new PlayerStateDiving()},
            {Player.State.CELEBRATE ,() =>new PlayerStateCelebrate()},
            {Player.State.MOURNING ,() =>new PlayerStateMourning()},
            
        };
    }

    public PlayerState GetFreshState(Player.State state)
    {
        if (!_states.ContainsKey(state))
        {
            throw new System.Exception("State does not exist");
        }
        return _states[state]();
    }
}
