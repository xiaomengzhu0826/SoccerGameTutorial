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
	private Node2D _rootParticles;
	private GpuParticles2D _runParticles;

	public Ball _ball;
	public Goal _ownGoal;
	public Goal _targetGoal;
	public float _Speed = 80;
	public float _Power = 70;
	public Vector2 _Heading = Vector2.Right;	
	public float _Height;
	public float _HeightVelocity;
	public Vector2 _KickoffPosition;
	public Role _Role;
	public string _Country;
	public Vector2 _SpawnPosition;
	public float _WeightOnDutySteering;
	public string _FullName;

	private readonly PlayerStateFactroy _stateFactory = new();
	private readonly AiBehaviourFactroy _aiBehaviourFactory = new();
	private PlayerState _currentState;
	private AiBehavior _currentAiBehavior;
	
	private SkinColor _skinColor;


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
		MOURNING,
		RESETING
	}

	public void Init(Vector2 contextPosition, Vector2 contextkickoffPosition, Ball contextBall, Goal contextOwnGoal, Goal contextTargetGoal, PlayerResource contextPlayerData, string contextCountry)
	{
		Position = contextPosition;
		_ball = contextBall;
		_ownGoal = contextOwnGoal;
		_targetGoal = contextTargetGoal;
		_Speed = contextPlayerData.Speed;
		_Power = contextPlayerData.Power;
		_FullName = contextPlayerData.FullName;
		_Role = contextPlayerData.Role;
		_skinColor = contextPlayerData.Skin;
		_Heading = (_targetGoal.Position.X < Position.X) ? Vector2.Left : Vector2.Right;
		_Country = contextCountry;
		_KickoffPosition = contextkickoffPosition;
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
		_rootParticles=GetNode<Node2D>("RootParticles");
		_runParticles=GetNode<GpuParticles2D>("%RunParticles");
		SetControlTexture();
		SetupAiBehavior();
		SetShaderProperties();

		_permanentDamageEmitter.Monitoring = _Role == Role.GOALIE;
		_goalieHandsCollider.Disabled = _Role != Role.GOALIE;

		_SpawnPosition = Position;

		_tackleDamageEmitterArea.BodyEntered += OnTacklePlayer;
		_permanentDamageEmitter.BodyEntered += OnTacklePlayer;
		SignalManager.Instance.OnTeamScored += OnTeamScored;
		SignalManager.Instance.OnGameOver += OnGameOver;

		var initialPosition = _Country == DataLoader.Instance._Countries[0] ? _KickoffPosition : _SpawnPosition;
		SwitchState(State.RESETING, PlayerStateData.Build().SetResetPosition(initialPosition));
	}

	public override void _ExitTree()
	{
		SignalManager.Instance.OnTeamScored -= OnTeamScored;
		SignalManager.Instance.OnGameOver -= OnGameOver;
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
		var countryColor = FindIndexFromName(_Country);
		playerShader.SetShaderParameter("team_color", countryColor);

	}

	private int FindIndexFromName(string name)
	{
		if (Enum.TryParse(_Country, true, out Country result))
		{
			return (int)result; // 枚举值本质就是索引
		}
		return 1; // 找不到
	}

	private void SetupAiBehavior()
	{
		_currentAiBehavior = _aiBehaviourFactory.GetAiBehavior(_Role);
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
		else if (velLehgth < _Speed * WALK_ANIM_THRESHOLD)
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
		if (_Height > 0)
		{
			_HeightVelocity -= GRAVITY * delta;
			_Height += _HeightVelocity;
			if (_Height <= 0)
			{
				_Height = 0;
			}
		}
		_playerSprite.Position = Vector2.Up * _Height;
	}

	public void SetHeading()
	{
		if (Velocity.X > 0)
		{
			_Heading = Vector2.Right;
		}
		else if (Velocity.X < 0)
		{
			_Heading = Vector2.Left;
		}
	}
	
	public void FaceTowardTargetGoal()
    {
        if (!IsFacingTargetGoal())
        {
            _Heading *= -1;
        }
    }

	private void FlipSprites()
	{
		//_playerSprite.FlipH = _heading == Vector2.Left ? true : false;
		if (_Heading == Vector2.Right)
		{
			_playerSprite.FlipH = false;
			_tackleDamageEmitterArea.Scale = new Vector2(1, 1);
			_opponentDetectionArea.Scale = new Vector2(1, 1);
			_rootParticles.Scale = new Vector2(1, 1);
		}
		else
		{
			_playerSprite.FlipH = true;
			_tackleDamageEmitterArea.Scale = new Vector2(-1, 1);
			_opponentDetectionArea.Scale = new Vector2(-1, 1);
			_rootParticles.Scale= new Vector2(-1, 1);
		}
	}

	public void SetControlScheme(ControlScheme scheme)
	{
		_controlScheme = scheme;
		SetControlTexture();
	}

	private void SetSpriteVisibility()
	{
		_controlSprite.Visible = HasBall() || _controlScheme != ControlScheme.CPU;
		_runParticles.Emitting = Velocity.Length() == _Speed;
	}

	public bool HasBall()
	{
		return _ball._carrier == this;
	}

	public bool IsReadyForKickoff()
	{
		return _currentState != null && _currentState.IsReadyForKickoff();
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
		return _Heading.Dot(directionToTargetGoal) > 0;
	}

	public bool CanCarryBall()
    {
		return _currentState != null && _currentState.CanCarryBall();
    }

	private void OnTacklePlayer(Node2D body)
	{
		Player player = (Player)body;
		if (player != this && player._Country != _Country && player == _ball._carrier)
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
		if (_Country == country)
		{
			SwitchState(State.MOURNING, null);
		}
		else
		{
			SwitchState(State.CELEBRATE, null);
		}
    }

	private void OnGameOver(string countryWinner)
    {
        if (_Country == countryWinner)
		{
			SwitchState(State.CELEBRATE, null);
		}
		else
		{
			SwitchState(State.MOURNING, null);
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
