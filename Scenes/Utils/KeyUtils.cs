using Godot;
using System;
using System.Collections.Generic;

public class KeyUtils 
{

    public enum Action { LEFT, RIGHT, UP, DOWN, SHOOT, PASS }

    public static readonly Dictionary<Player.ControlScheme, Dictionary<Action, string>> ACTION_MAP = new()
    {
        {
            Player.ControlScheme.P1, new Dictionary<Action, string>
            {
                { Action.LEFT, "p1_left" },
                { Action.RIGHT, "p1_right" },
                { Action.UP, "p1_up" },
                { Action.DOWN, "p1_down" },
                { Action.PASS, "p1_pass" },
                { Action.SHOOT, "p1_shoot" },
            }
        },
        {
            Player.ControlScheme.P2, new Dictionary<Action, string>
            {
                { Action.LEFT, "p2_left" },
                { Action.RIGHT, "p2_right" },
                { Action.UP, "p2_up" },
                { Action.DOWN, "p2_down" },
                { Action.PASS, "p2_pass" },
                { Action.SHOOT, "p2_shoot" },
            }
        },
    };

    public static Vector2 GetInputVector(Player.ControlScheme scheme)
    {
        Dictionary<Action, string> map = ACTION_MAP[scheme];
        return Input.GetVector(map[Action.LEFT], map[Action.RIGHT], map[Action.UP], map[Action.DOWN]);
    }

    public static bool IsActionPressed(Player.ControlScheme scheme, Action action)
    {
        return Input.IsActionPressed(ACTION_MAP[scheme][action]);
    }

    public static bool IsActionJustPressed(Player.ControlScheme scheme, Action action)
    {
        return Input.IsActionJustPressed(ACTION_MAP[scheme][action]);
    }
    
    public static bool IsActionJustReleased(Player.ControlScheme scheme,Action action)
    {
        return Input.IsActionJustReleased(ACTION_MAP[scheme][action]);
    }
}
