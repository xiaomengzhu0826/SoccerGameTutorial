using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public static class Extensions
{

    public static int Find(this Player.Country _, string name)
    {
        if (Enum.TryParse(typeof(Player.Country), name, true, out var result))
        {
            return (int)result; // 找到 → 返回枚举的 int 值
        }
        return -1; // 找不到 → 返回 -1
    }

    public static IEnumerable<Player> FilterCpuAndNoGoalkeeper(this IEnumerable<Player> players)
    {
        return players.Where(p => p._controlScheme == Player.ControlScheme.CPU &&
                                  p._role != Player.Role.GOALIE);
    }

    public static void SortByDistanceTo(this List<Player> players, Vector2 targetPosition)
    {
        players.Sort((a, b) =>
            a.Position.DistanceTo(targetPosition)
            .CompareTo(b.Position.DistanceTo(targetPosition))
        );
    }

}
