using Godot;
using System;

public partial class PlayerStateData : Resource
{
    public Vector2 HurtDirection;
    public Vector2 ShotDirection;
    public float ShotPower;
    public Player PassTarget;

    public static PlayerStateData Build()
    {
        return new PlayerStateData();
    }

    public PlayerStateData SetShotDirection(Vector2 direction)
    {
        ShotDirection = direction;
        return this;
    }

    public PlayerStateData SetShotPower(float power)
    {
        ShotPower = power;
        return this;
    }

    public PlayerStateData SetHurtDirection(Vector2 direciton)
    {
        HurtDirection = direciton;
        return this;
    }

    public PlayerStateData SetPassTarget(Player player)
    {
        PassTarget = player;
        return this;
    }

}
