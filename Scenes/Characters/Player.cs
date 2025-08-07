using Godot;
using System;
using System.Collections.Generic;

public partial class Player : CharacterBody2D
{

	[Export] public float _speed = 80;
	[Export] public float _power = 70;
	[Export] public ControlScheme _controlScheme;
	[Export] public Ball _ball;

	private AnimationPlayer _animationPlayer;
	private Sprite2D _playerSprite;
	private Sprite2D _controlSprite;
	private Area2D _teammateDetectionArea;

	public Vector2 _heading = Vector2.Right;
	private PlayerState _currentState;
	private PlayerStateFactroy _stateFactory = new();

	public readonly Dictionary<ControlScheme, Texture2D> CONTROL_SCHEME_MAP = new()
	{
		{ControlScheme.CPU,GD.Load<Texture2D>("res://Assets/art/props/cpu.png")},
		{ControlScheme.P1,GD.Load<Texture2D>("res://Assets/art/props/1p.png")},
		{ControlScheme.P2,GD.Load<Texture2D>("res://Assets/art/props/2p.png")},
	};

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
		_controlSprite = GetNode<Sprite2D>("%ControlSprite");
		_teammateDetectionArea = GetNode<Area2D>("TeammateDetection");

		SetControlTexture();

		SwitchState(State.MOVING,null);
	}

	public override void _Process(double delta)
	{
		FlipSprites();
		SetSpriteVisibility();
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

	private void SetSpriteVisibility()
	{
		_controlSprite.Visible = HasBall() || _controlScheme != ControlScheme.CPU;
	}

	public bool HasBall()
	{
		return _ball._carrier == this;
	}

	public void SetControlTexture()
	{
		_controlSprite.Texture = CONTROL_SCHEME_MAP[_controlScheme];
	}

	public void OnAnimationCompelete()
	{
		if (_currentState != null)
		{
			_currentState.OnAnimationCompelete();
		}
	}
}
