using Godot;
using System;

public partial class Player : CharacterBody2D
{
	[Export] public float _speed = 80;
	[Export] public float _power = 70;
	[Export] public ControlScheme _controlScheme;
	[Export] public Ball _ball;

	private AnimationPlayer _animationPlayer;
	private Sprite2D _playerSprite;
	private Area2D _teammateDetectionArea;

	public Vector2 _heading = Vector2.Right;
	private PlayerState _currentState;
	private PlayerStateFactroy _stateFactory = new();

	public enum ControlScheme
	{
		CPU,
		P1,
		P2
	}

	public enum State
	{
		MOVING,
		TACKLING,
		RECOVERING,
		PREPPING_SHOT,
		SHOOTING,
		PASSING
	}

	public override void _Ready()
	{
		_animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
		_playerSprite = GetNode<Sprite2D>("PlayerSprite");
		_teammateDetectionArea = GetNode<Area2D>("TeammateDetection");

		SwitchState(State.MOVING,null);
	}

	public override void _Process(double delta)
	{
		FlipSprites();
		MoveAndSlide();
	}

	private void SwitchState(State state, PlayerStateData stateData)
	{
		if (_currentState != null)
		{
			_currentState.OnStateTransitionRequest -= SwitchState;
			_currentState.QueueFree();
		}
		_currentState = _stateFactory.GetFreshState(state);
		_currentState.Setup(this,stateData, _animationPlayer,_ball,_teammateDetectionArea);
		_currentState.OnStateTransitionRequest += SwitchState;
		_currentState.Name = "PlayerStateMachine:" + state.ToString();
		CallDeferred("add_child", _currentState);
	}


    public void SetMovementAnimation()
	{
		if (Velocity.Length() > 0)
		{
			_animationPlayer.Play("run");
		}
		else
		{
			_animationPlayer.Play("idle");
		}
	}

	public void SetHeading()
	{
		if (Velocity.X > 0)
		{
			_heading = Vector2.Right;
		}
		else if (Velocity.X < 0)
		{
			_heading = Vector2.Left;
		}
	}

	private void FlipSprites()
	{
		_playerSprite.FlipH = _heading == Vector2.Left ? true : false;
	}

	public bool HasBall()
	{
		return _ball._carrier == this;
	}

	public void OnAnimationCompelete()
	{
		if (_currentState != null)
		{
			_currentState.OnAnimationCompelete();
		}
	}
}
