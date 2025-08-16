using Godot;
using System;
using System.Collections.Generic;

public partial class Player : CharacterBody2D
{
	[Signal] public delegate void OnSwapRequestEventHandler(Player player);
	
	// public static void EmitOnSwapRequest(Player player)
	// {
	// 	Instance.EmitSignal(SignalName.OnSwapRequest,player);
	// }

	[Export] public ControlScheme _controlScheme;

	public Ball _ball;
	public Goal _ownGoal;
	public Goal _targetGoal;

	private static readonly float GRAVITY = 8.0f;
	private static readonly float WALK_ANIM_THRESHOLD = 0.6f;

	private readonly float BALL_CONTROL_HEIGHT_MAX = 10.0f;

	private AnimationPlayer _animationPlayer;
	private Sprite2D _playerSprite;
	private Sprite2D _controlSprite;
	private Area2D _teammateDetectionArea;
	private Area2D _ballDetectionArea;
	private Area2D _tackleDamageEmitterArea;
	private Area2D _opponentDetectionArea;
	private Area2D _permanentDamageEmitter;
	private CollisionShape2D _goalieHandsCollider;

	public float _speed = 80;
	public float _power = 70;
	public Vector2 _heading = Vector2.Right;	
	public float _height;
	public float _heightVelocity;

	private readonly PlayerStateFactroy _stateFactory = new();
	private readonly AiBehaviourFactroy _aiBehaviourFactory = new();
	private PlayerState _currentState;
	private AiBehavior _currentAiBehavior;

	private string _fullName;
	private SkinColor _skinColor;
	public Role _role;
	public string _country;
	public Vector2 _spawnPosition;
	public float _weightOnDutySteering;

	public readonly Dictionary<ControlScheme, Texture2D> CONTROL_SCHEME_MAP = new()
	{
		{ControlScheme.CPU,GD.Load<Texture2D>("res://Assets/art/props/cpu.png")},
		{ControlScheme.P1,GD.Load<Texture2D>("res://Assets/art/props/1p.png")},
		{ControlScheme.P2,GD.Load<Texture2D>("res://Assets/art/props/2p.png")},
	};

	public enum Country
	{
		DEFAULT,
		FRANCE,
		ARGENTINA,
		BRAZIL,
		ENGLAND,
		GERMANY,
		ITALY,
		SPAIN,
		USA
	}

	public enum ControlScheme
	{
		CPU,
		P1,
		P2
	}

	public enum Role
	{
		GOALIE,
		DEFENSE,
		MIDFIELD,
		OFFENSE
	}

	public enum SkinColor
	{
		LIGHT,
		MEDIUM,
		DARK
	}

	public enum State
	{
		MOVING,
		TACKLING,
		RECOVERING,
		PREPPING_SHOT,
		SHOOTING,
		PASSING,
		HEADER,
		VOLLEY_KICK,
		BICYCLE_KICK,
		CHEST_CONTROL,
		HURT,
		DIVING,
		CELEBRATE,
		MOURNING
	}

	public void Init(Vector2 contextPosition, Ball contextBall, Goal contextOwnGoal, Goal contextTargetGoal, PlayerResource contextPlayerData, string contextCountry)
	{
		Position = contextPosition;
		_ball = contextBall;
		_ownGoal = contextOwnGoal;
		_targetGoal = contextTargetGoal;
		_speed = contextPlayerData.Speed;
		_power = contextPlayerData.Power;
		_fullName = contextPlayerData.FullName;
		_role = contextPlayerData.Role;
		_skinColor = contextPlayerData.Skin;
		_heading = (_targetGoal.Position.X < Position.X) ? Vector2.Left : Vector2.Right;
		_country = contextCountry;
	}

	public override void _Ready()
	{

		_animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
		_playerSprite = GetNode<Sprite2D>("PlayerSprite");
		_controlSprite = GetNode<Sprite2D>("%ControlSprite");
		_teammateDetectionArea = GetNode<Area2D>("TeammateDetection");
		_ballDetectionArea = GetNode<Area2D>("BallDetectionArea");
		_tackleDamageEmitterArea = GetNode<Area2D>("TackleDamageEmitterArea");
		_opponentDetectionArea = GetNode<Area2D>("OpponentDetectionArea");
		_permanentDamageEmitter = GetNode<Area2D>("PermanentDamageEmitter");
		_goalieHandsCollider = GetNode<CollisionShape2D>("%GoalieHandsCollider");
		SetControlTexture();
		SetupAiBehavior();
		SwitchState(State.MOVING, null);
		SetShaderProperties();

		_permanentDamageEmitter.Monitoring = _role == Role.GOALIE;
		_goalieHandsCollider.Disabled = _role != Role.GOALIE;

		_spawnPosition = Position;

		_tackleDamageEmitterArea.BodyEntered += OnTacklePlayer;
		_permanentDamageEmitter.BodyEntered += OnTacklePlayer;
		SignalManager.Instance.OnTeamScored += OnTeamScored;
	}



    public override void _Process(double delta)
	{
		FlipSprites();
		SetSpriteVisibility();
		ProcessGravity((float)delta);
		MoveAndSlide();
	}

	private void SetShaderProperties()
	{
		var playerShader = _playerSprite.Material as ShaderMaterial;
		playerShader.SetShaderParameter("skin_color", (int)_skinColor);
		var countryColor = FindIndexFromName(_country);
		playerShader.SetShaderParameter("team_color", countryColor);

	}

	private int FindIndexFromName(string name)
	{
		if (Enum.TryParse(_country, true, out Country result))
		{
			return (int)result; // 枚举值本质就是索引
		}
		return 1; // 找不到
	}

	private void SetupAiBehavior()
	{
		_currentAiBehavior = _aiBehaviourFactory.GetAiBehavior(_role);
		_currentAiBehavior.Setup(this, _ball,_opponentDetectionArea,_teammateDetectionArea);
		_currentAiBehavior.Name = "AI Behavior";
		AddChild(_currentAiBehavior);
	}

	public void SwitchState(State state, PlayerStateData stateData)
	{
		if (_currentState != null)
		{
			_currentState.OnStateTransitionRequest -= SwitchState;
			_currentState.QueueFree();
		}
		_currentState = _stateFactory.GetFreshState(state);
		_currentState.Setup(this, stateData, _animationPlayer, _ball,
							_teammateDetectionArea, _ballDetectionArea,
							_ownGoal, _targetGoal, _tackleDamageEmitterArea,_currentAiBehavior);
		_currentState.OnStateTransitionRequest += SwitchState;
		_currentState.Name = "PlayerStateMachine:" + state.ToString();
		CallDeferred("add_child", _currentState);
	}


	public void SetMovementAnimation()
	{
		var velLehgth = Velocity.Length();
		if (velLehgth < 1)
		{
			_animationPlayer.Play("idle");
		}
		else if (velLehgth < _speed * WALK_ANIM_THRESHOLD)
		{
			_animationPlayer.Play("walk");
		}
		else
		{
			_animationPlayer.Play("run");
		}
	}

	private void ProcessGravity(float delta)
	{
		if (_height > 0)
		{
			_heightVelocity -= GRAVITY * delta;
			_height += _heightVelocity;
			if (_height <= 0)
			{
				_height = 0;
			}
		}
		_playerSprite.Position = Vector2.Up * _height;
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
		//_playerSprite.FlipH = _heading == Vector2.Left ? true : false;
		if (_heading == Vector2.Right)
		{
			_playerSprite.FlipH = false;
			_tackleDamageEmitterArea.Scale = new Vector2(1, 1);
			_opponentDetectionArea.Scale = new Vector2(1, 1);
		}
		else
		{
			_playerSprite.FlipH = true;
			_tackleDamageEmitterArea.Scale = new Vector2(-1, 1);
			_opponentDetectionArea.Scale=new Vector2(-1, 1);
		}
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

	public void GetPassRequest(Player player)
	{
		if (_ball._carrier == this && _currentState != null && _currentState.CanPass())
		{
			SwitchState(State.PASSING, PlayerStateData.Build().SetPassTarget(player));
		}
	}

	public bool IsFacingTargetGoal()
	{
		var directionToTargetGoal = Position.DirectionTo(_targetGoal.Position);
		return _heading.Dot(directionToTargetGoal) > 0;
	}

	public bool CanCarryBall()
    {
		return _currentState != null && _currentState.CanCarryBall();
    }

	private void OnTacklePlayer(Node2D body)
	{
		Player player = (Player)body;
		if (player != this && player._country != _country && player == _ball._carrier)
		{
			player.GetHurt(Position.DirectionTo(player.Position));
		}
	}

    private void GetHurt(Vector2 hurtOrigin)
    {
		SwitchState(State.HURT, PlayerStateData.Build().SetHurtDirection(hurtOrigin));
    }

    public void OnAnimationCompelete()
	{
		if (_currentState != null)
		{
			_currentState.OnAnimationCompelete();
		}
	}

	private void OnTeamScored(string country)
	{
		if (_country == country)
		{
			SwitchState(State.MOURNING, null);
		}
		else
		{
			SwitchState(State.CELEBRATE, null);
		}
    }

	public void ControlBall()
	{
		if (_ball._height > BALL_CONTROL_HEIGHT_MAX)
		{
			SwitchState(State.CHEST_CONTROL, null);
		}
	}


}
