using Godot;
using System;

public partial class PlayerStateData : Resource
{
    public Vector2 _HurtDirection;
    public Vector2 _ShotDirection;
    public Vector2 _ResetPosition;
    public float _ShotPower;
    public Player _PassTarget;

    public static PlayerStateData Build()
    {
        return new PlayerStateData();
    }

    public PlayerStateData SetShotDirection(Vector2 direction)
    {
        _ShotDirection = direction;
        return this;
    }

    public PlayerStateData SetShotPower(float power)
    {
        _ShotPower = power;
        return this;
    }

    public PlayerStateData SetHurtDirection(Vector2 direciton)
    {
        _HurtDirection = direciton;
        return this;
    }

    public PlayerStateData SetPassTarget(Player player)
    {
        _PassTarget = player;
        return this;
    }

    public PlayerStateData SetResetPosition(Vector2 position)
    {
        _ResetPosition = position;
        return this;
    }

}
