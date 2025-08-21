using Godot;
using System;
using System.Collections.Generic;

public partial class Ball : AnimatableBody2D
{
	private static readonly float TUMBLE_HEIGHT_VELOCITY = 1.0f;
	private static readonly int DURATION_TUMBLE_LOCK = 200;
	private static readonly int DURATION_PASS_LOCK = 500;
	private static readonly float KICKOFF_PASS_DISTANCE = 30.0f;

	private Sprite2D _ballSprite;
	private Area2D _playerDetectionArea;
	private AnimationPlayer _animationPlayer;
	private RayCast2D _scoringRayCast;
	private GpuParticles2D _shotParticles;
	private Area2D _playerProximityArea;

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
	private Vector2 _spawnPosition;

	public enum State
	{
		CARRIED,
		FREEFORM,
		SHOT
	}


	public override void _Ready()
	{
		AddToGroup("ball");
		//AddToGroup(nameof(Ball));
		_playerDetectionArea = GetNode<Area2D>("PlayerDetectionArea");
		_animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
		_ballSprite = GetNode<Sprite2D>("BallSprite");
		_scoringRayCast = GetNode<RayCast2D>("ScoringRayCast");
		_shotParticles = GetNode<GpuParticles2D>("ShotParticles");
		_playerProximityArea = GetNode<Area2D>("PlayerProximityArea");

		_spawnPosition = Position;
		SignalManager.Instance.OnTeamReset += OnTeamReset;
		SignalManager.Instance.OnKickoffStarted += OnKickoffStarted;

		SwitchState(State.FREEFORM, null);
		
	}

	public override void _ExitTree()
	{
		SignalManager.Instance.OnTeamReset -= OnTeamReset;
		SignalManager.Instance.OnKickoffStarted -= OnKickoffStarted;
	}


	public override void _Process(double delta)
	{
		_ballSprite.Position = Vector2.Up * _height;
		_scoringRayCast.Rotation = _velocity.Angle();
	}


	public void SwitchState(State state, BallStateData ballStateData)
	{
		if (_currentState != null)
		{
			_currentState.OnStateTransitionRequest -= SwitchState;
			_currentState.QueueFree();
		}
		_currentState = _stateFactory.GetFreshState(state);
		_currentState.Setup(this, ballStateData, _playerDetectionArea, _carrier, _animationPlayer, _ballSprite,_shotParticles);
		_currentState.OnStateTransitionRequest += SwitchState;
		_currentState.Name = "BallStateMachine:" + state.ToString();
		CallDeferred("add_child", _currentState);
	}

	public void Shoot(Vector2 shotVelocity)
	{
		_velocity = shotVelocity;
		_carrier = null;
		SwitchState(State.SHOT, null);
	}

	public void Tumble(Vector2 tumbleVelocity)
	{
		_velocity = tumbleVelocity;
		_carrier = null;
		_heightVelocity = TUMBLE_HEIGHT_VELOCITY;
		SwitchState(State.FREEFORM, BallStateData.Build().SetLockDuration(DURATION_TUMBLE_LOCK));
	}

	public void PassTo(Vector2 destination,int lockDuration=500)
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
		SwitchState(State.FREEFORM, BallStateData.Build().SetLockDuration(lockDuration));
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

	public bool CanAirConnect(float airConnectMinHeight, float airConnectMaxHeight)
	{
		return _height >= airConnectMinHeight && _height <= airConnectMaxHeight;
	}

	public bool IsHeadedForScoringArea(Area2D scoringArea)
	{
		if (!_scoringRayCast.IsColliding())
		{
			return false;
		}
		return _scoringRayCast.GetCollider() == scoringArea;
	}

	public int GetProximityTeammatesCount(string country)
	{
		var players = _playerProximityArea.GetOverlappingBodies() ;
		return players.FilterWithTeammatesCount(country);
	} 

	private void OnTeamReset()
	{
		Position = _spawnPosition;
		_velocity = Vector2.Zero;
		SwitchState(State.FREEFORM, null);
	}
	
	private void OnKickoffStarted()
    {
		PassTo(_spawnPosition + Vector2.Down * KICKOFF_PASS_DISTANCE,0);
    }
}
