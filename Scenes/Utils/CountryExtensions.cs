using Godot;
using System;

public static class CountryExtensions
{

    public static int Find(this Player.Country _, string name)
    {
        if (Enum.TryParse(typeof(Player.Country), name, true, out var result))
        {
            return (int)result; // 找到 → 返回枚举的 int 值
        }
        return -1; // 找不到 → 返回 -1
    }
}
