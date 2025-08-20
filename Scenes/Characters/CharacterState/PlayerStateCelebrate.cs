using Godot;
using System;

public partial class PlayerStateCelebrate : PlayerState
{
    private static readonly float CELEBRATE_HEIGHT = 1f;
    private static readonly float AIR_FRICTION = 35.0f;

    private int _initialDelay = GD.RandRange(200, 500);
	private float _timeSinceCelebrating = Time.GetTicksMsec();

    public override void _EnterTree()
    {
        SignalManager.Instance.OnTeamReset += OnTeamReset;
    }

    public override void _ExitTree()
    {
        SignalManager.Instance.OnTeamReset -= OnTeamReset;
    }

    public override void _Process(double delta)
    {
        if (_player._Height == 0 && Time.GetTicksMsec()-_timeSinceCelebrating>_initialDelay)
        {
            Celebrate();
        }
        _player.Velocity = _player.Velocity.MoveToward(Vector2.Zero, (float)delta * AIR_FRICTION);
    }

    private void Celebrate()
    {
        _animationPlayer.Play("celebrate");
        _player._Height = 0.1f;
        _player._HeightVelocity = CELEBRATE_HEIGHT;
    }

    private void OnTeamReset()
    {
        EmitSignal(PlayerState.SignalName.OnStateTransitionRequest, (int)Player.State.RESETING, PlayerStateData.Build().SetResetPosition(_player._SpawnPosition));
    }

    
}
