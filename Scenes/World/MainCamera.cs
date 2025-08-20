using Godot;
using System;

public partial class MainCamera : Camera2D
{
	private static readonly float DISTANCE_TARGET = 100.0f;
	private static readonly int DURATION_SHAKE = 120;
	private static readonly int SHAKE_INTENSITY = 5;
	private static readonly int SMOOTHING_BALL_CARRIED = 1;
	private static readonly int SMOOTHING_BALL_DEFAULT = 6;

	private Ball _ball;
	private float _timeStartShake = Time.GetTicksMsec();
	private bool _isShaking;

	public override void _Ready()
	{
		CallDeferred(nameof(FindBall));
		SignalManager.Instance.OnImpactReceived += OnImpactReceived;
	}

	public override void _ExitTree()
	{
		SignalManager.Instance.OnImpactReceived -= OnImpactReceived;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (_ball._carrier != null)
		{
			Position = _ball._carrier.Position + _ball._carrier._Heading * DISTANCE_TARGET;
			PositionSmoothingSpeed = SMOOTHING_BALL_CARRIED;
		}
		else
		{
			Position = _ball.Position;
			PositionSmoothingSpeed = SMOOTHING_BALL_DEFAULT;
		}

		if (_isShaking && Time.GetTicksMsec() - _timeStartShake < DURATION_SHAKE)
		{
			Offset = new Vector2(GD.RandRange(-SHAKE_INTENSITY, SHAKE_INTENSITY), GD.RandRange(-SHAKE_INTENSITY, SHAKE_INTENSITY));
		}
		else
		{
			_isShaking = false;
			Offset = Vector2.Zero;
		}
	}

	private void FindBall()
	{
		var balls = GetTree().GetNodesInGroup("ball");
		if (balls.Count > 0)
		{
			_ball = balls[0] as Ball;
		}
	}

	private void OnImpactReceived(Vector2 impactPosition, bool isHighImpact)
	{
		if (isHighImpact)
		{
			_isShaking = true;
			_timeStartShake=Time.GetTicksMsec();
		}
    }
}
