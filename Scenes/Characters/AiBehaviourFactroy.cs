using Godot;
using System;
using System.Collections.Generic;

public class AiBehaviourFactroy 
{
    public Dictionary<Player.Role,  Func<AiBehavior>> _roles;

    public AiBehaviourFactroy()
    {
        _roles = new()
        {
            {Player.Role.GOALIE, () => new AiBehaviorGoalie() },
            {Player.Role.DEFENSE, () => new AiBehaviorField() },
            {Player.Role.MIDFIELD, () => new AiBehaviorField() },
            {Player.Role.OFFENSE, () => new AiBehaviorField() },
        };
    }

    public AiBehavior GetAiBehavior(Player.Role role)
    {
        if (!_roles.ContainsKey(role))
        {
            throw new System.Exception("role does not exist");
        }
        return _roles[role]();
    }
}
