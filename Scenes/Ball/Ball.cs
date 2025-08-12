using Godot;
using System;

public partial class Ball : AnimatableBody2D
{
	private Sprite2D _ballSprite;
	private Area2D _playerDetectionArea;
	private AnimationPlayer _animationPlayer;

	public static readonly float BOUNCINESS = 0.8f;
	public static readonly float DISTANCE_HIGH_PASS = 130.0f;
	public static readonly float FRICTIONAIR = 35.0f;
	public static readonly float FRICTIONGROUND = 250.0f;

	public Player _carrier;
	public Vector2 _velocity;
	public float _height;
	public float _heightVelocity;
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
		//AddToGroup(nameof(Ball));
		_playerDetectionArea = GetNode<Area2D>("PlayerDetectionArea");
		_animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
		_ballSprite = GetNode<Sprite2D>("BallSprite");

		SwitchState(State.FREEFORM);
	}

	public override void _Process(double delta)
	{
		_ballSprite.Position = Vector2.Up * _height;
	}


	public void SwitchState(State state)
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
		SwitchState(State.SHOT);
	}

	public void PassTo(Vector2 destination)
	{
		var direction = GlobalPosition.DirectionTo(destination);
		var distance = GlobalPosition.DistanceTo(destination);
		var intensity = Mathf.Sqrt(2 * distance * FRICTIONGROUND);
		_velocity = intensity * direction;
		if (distance > DISTANCE_HIGH_PASS)
		{
			_heightVelocity = BallState.GRAVITY * distance / (1.8f * intensity);
		}

		_carrier = null;
		SwitchState(State.FREEFORM);
	}

	public void Stop()
	{
		_velocity = Vector2.Zero;
	}

	public bool IsFreeform()
	{
		return _currentState != null && _currentState is BallStateFreeform;
	}

	public bool CanAirInteract()
	{
		return _currentState != null && _currentState.CanAirInteract();
	}

	public bool CanAirConnect(float airConnectMinHeight,float airConnectMaxHeight)
	{
		return _height >= airConnectMinHeight && _height <= airConnectMaxHeight;
	}
}
