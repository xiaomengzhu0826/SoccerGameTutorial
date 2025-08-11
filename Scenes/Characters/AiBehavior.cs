using Godot;
using System;

public partial class AiBehavior : Node
{
    private static readonly int DURATION_AI_TICK_FREQUENCY = 200;

    public Ball _ball;
    public Player _player;
    private float _timeSinceLastAiTick = Time.GetTicksMsec();

    public void Setup(Player contextPlayer, Ball contextBall)
    {
        _player = contextPlayer;
        _ball = contextBall;

        _timeSinceLastAiTick = Time.GetTicksMsec() + (float)GD.RandRange(0, DURATION_AI_TICK_FREQUENCY);
    }

    public void ProcessAi()
    {
        if (Time.GetTicksMsec() - _timeSinceLastAiTick > DURATION_AI_TICK_FREQUENCY)
        {
            _timeSinceLastAiTick = Time.GetTicksMsec();
            PerformAiMovement();
            PerformAiDecisions();
        }

    }

    private void PerformAiMovement()
    {
        var totalSteeringForce = Vector2.Zero;
        totalSteeringForce += GetOndutySteeringForce();
        totalSteeringForce = totalSteeringForce.LimitLength(1.0f);
        _player.Velocity = totalSteeringForce * _player._speed;
    }

    private void PerformAiDecisions()
    {

    }

    public Vector2 GetOndutySteeringForce()
    {
        return _player._weightOnDutySteering * _player.Position.DirectionTo(_ball.Position);
    }
}
