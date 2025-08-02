using Godot;
using System;

public partial class Player : CharacterBody2D
{
	[Export] private float _speed;
	[Export] private ControlScheme _controlScheme;

	private AnimationPlayer _animationPlayer;
	private Sprite2D _playerSprite;

	private Vector2 _heading = Vector2.Right;

	public enum ControlScheme
	{
		CPU,
		P1,
		P2
	}

	public override void _Ready()
	{
		_animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
		_playerSprite = GetNode<Sprite2D>("PlayerSprite");
	}

	public override void _Process(double delta)
	{
		if (_controlScheme == ControlScheme.CPU)
		{

		}
		else
		{
			HandleHumanMovement();
		}
		SetMovementAnimation();
		SetHeading();
		FlipSprites();
		MoveAndSlide();
	}

	private void HandleHumanMovement()
	{
		var direction = KeyUtils.GetInputVector(_controlScheme);
		Velocity = direction * _speed;
	}

	private void SetMovementAnimation()
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

	private void SetHeading()
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
}
