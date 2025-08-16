using Godot;
using System;

public partial class BallStateData:Resource
{
    public int LockDuration;

    public static BallStateData Build()
    {
        return new BallStateData();
    }

    public BallStateData SetLockDuration(int duration)
    {
        LockDuration = duration;
        return this;
    }
}
