using Godot;
using System;

public partial class Ball : AnimatableBody2D
{
	[Export] private Vector2 _offsetFromPlayer;

	private Area2D _playerDetectionArea;

	public Player _carrier;

	private Vector2 _velocity = Vector2.Zero;
	private BallState _currentState;
	private BallStateFactory _stateFactory = new();
	

	public enum State
	{
		CARRIED,
		FREEFORM,
		SHOT
	}


	public override void _Ready()
	{
		_playerDetectionArea = GetNode<Area2D>("PlayerDetectionArea");

		SwitchState(State.FREEFORM);
	}

	public override void _Process(double delta)
	{
	}

	private void SwitchState(State state)
	{
		if (_currentState != null)
		{
			_currentState.OnStateTransitionRequest -= SwitchState;
			_currentState.QueueFree();
		}
		_currentState = _stateFactory.GetFreshState(state);
		_currentState.Setup(this,_playerDetectionArea,_carrier);
		_currentState.OnStateTransitionRequest += SwitchState;
		_currentState.Name = "BallStateMachine:" + state.ToString();
		CallDeferred("add_child", _currentState);
	}
}
