using Godot;
using System;

public partial class PlayerStatePreppingShot : PlayerState
{
    private readonly float DURATION_MAX_BONUS = 1000.0f;
    private readonly float EASE_REWARD_FACTOR = 2.0f;

    private float _timeStartShot;
    private Vector2 _shot_direction;

    public override void _EnterTree()
    {
        _animationPlayer.Play("prep_kick");
        _player.Velocity = Vector2.Zero;
        _timeStartShot = Time.GetTicksMsec();
        _shot_direction = _player._heading;
    }

    public override void _Process(double delta)
    {
        _shot_direction += KeyUtils.GetInputVector(_player._controlScheme) * (float)delta;
        if (KeyUtils.IsActionJustReleased(_player._controlScheme, KeyUtils.Action.SHOOT))
        {
            var duration_press = Mathf.Clamp(Time.GetTicksMsec() - _timeStartShot, 0.0, DURATION_MAX_BONUS);
            var ease_time = duration_press / DURATION_MAX_BONUS;
            float bonus = (float)Mathf.Ease(ease_time, EASE_REWARD_FACTOR);
            float shot_power = _player._power * (1 + bonus);
            _shot_direction = _shot_direction.Normalized();
            // var stateData = new PlayerStateData()
            // {
            //     ShotPower = shot_power,
            //     ShotDirection = _shot_direction
            // };
            var stateData = PlayerStateData.Build().SetShotDirection(_shot_direction).SetShotPower(shot_power);
            EmitSignal(PlayerState.SignalName.OnStateTransitionRequest, (int)Player.State.SHOOTING, stateData);
        }
    }
}
