using Godot;
using System;

public partial class Ball : AnimatableBody2D
{

	private Sprite2D _ballSprite;
	private Area2D _playerDetectionArea;
	private AnimationPlayer _animationPlayer;

	public Player _carrier;
	public Vector2 _velocity = Vector2.Zero;
	public float _height;
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
		_animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
		_ballSprite = GetNode<Sprite2D>("BallSprite");

		SwitchState(State.FREEFORM);
	}
	
	public override void _Process(double delta)
	{
		_ballSprite.Position = Vector2.Up * _height;
	}


	private void SwitchState(State state)
	{
		if (_currentState != null)
		{
			_currentState.OnStateTransitionRequest -= SwitchState;
			_currentState.QueueFree();
		}
		_currentState = _stateFactory.GetFreshState(state);
		_currentState.Setup(this, _playerDetectionArea, _carrier, _animationPlayer, _ballSprite);
		_currentState.OnStateTransitionRequest += SwitchState;
		_currentState.Name = "BallStateMachine:" + state.ToString();
		CallDeferred("add_child", _currentState);
	}

	public void Shoot(Vector2 shotVelocity)
	{
		_velocity = shotVelocity;
		_carrier = null;
		SwitchState(Ball.State.SHOT);
	}
}
