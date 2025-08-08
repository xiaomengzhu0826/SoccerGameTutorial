using Godot;
using System;

public partial class MainCamera : Camera2D
{
	private readonly float DISTANCE_TARGET = 100.0f;
	private readonly int SMOOTHING_BALL_CARRIED = 1;
	private readonly int SMOOTHING_BALL_DEFAULT = 6;

	[Export] private Ball _ball;

	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (_ball._carrier != null)
		{
			Position = _ball._carrier.Position + _ball._carrier._heading * DISTANCE_TARGET;
			PositionSmoothingSpeed = SMOOTHING_BALL_CARRIED;
		}
		else
		{
			Position = _ball.Position;
			PositionSmoothingSpeed = SMOOTHING_BALL_DEFAULT;
		}
	}
}
