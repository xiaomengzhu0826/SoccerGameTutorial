using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class ActorsContainer : Node2D
{
	private static readonly PackedScene PLAYER_PREFAB = GD.Load<PackedScene>("res://Scenes/Characters/Player.tscn");
	private static readonly int DURATION_WEIGHT_CACHE = 200;

	[Export] private Ball _ball;
	[Export] private Goal _goalHome;
	[Export] private Goal _goalAway;


	private Node2D _spawns;
	private Node2D _kickOffs;

	private List<Player> _squadHome = new();
	private List<Player> _squadAway = new();
	private float _timeSinceLastCacheRefresh = Time.GetTicksMsec();
	private bool _isCheckingForKickoffReadiness;

	public override void _Ready()
	{
		_spawns = GetNode<Node2D>("Spawns");
		_kickOffs = GetNode<Node2D>("KickOffs");

		_squadHome = SpawnPlayer(GameManager.Instance._Countries[0], _goalHome);
		_spawns.Scale = new Vector2(-1, 1);
		_squadAway = SpawnPlayer(GameManager.Instance._Countries[1], _goalAway);
		_kickOffs.Scale = new Vector2(-1, 1);
		_goalHome.Initialize(GameManager.Instance._Countries[0]);
		_goalAway.Initialize(GameManager.Instance._Countries[1]);

		// foreach (var item in GetChildren())
		// {
		// 	if (item is Player player && item.GetIndex() == 8)
		// 	{
		// 		player._controlScheme = Player.ControlScheme.P1;
		// 		player.SetControlTexture();
		// 	}
		// }
		SetupControlSchemes();

		SignalManager.Instance.OnTeamReset += OnTeamReset;
	}



	public override void _Process(double delta)
	{
		if (Time.GetTicksMsec() - _timeSinceLastCacheRefresh > DURATION_WEIGHT_CACHE)
		{
			_timeSinceLastCacheRefresh = Time.GetTicksMsec();
			SetOnDutyWeights();
		}
		if (_isCheckingForKickoffReadiness)
		{
			CheckingForKickoffReadiness();
		}
	}

	private List<Player> SpawnPlayer(string country, Goal ownGoal)
	{
		List<Player> playerNodes = new();
		var players = DataLoader.Instance.GetSquad(country);
		Goal targetGoal = (_goalHome == ownGoal) ? _goalAway : _goalHome;
		for (int i = 0; i < players.Count; i++)
		{
			Node2D child = (Node2D)_spawns.GetChild(i);
			var playerPosition = child.GlobalPosition;
			PlayerResource playerData = players[i];
			var kickoffPosition = playerPosition;
			if (i > 3)
			{
				var offensekickoffPosition = _kickOffs.GetChild(i - 4) as Node2D;
				kickoffPosition = offensekickoffPosition.GlobalPosition;
			}
			var player = SpawnPlayer(playerPosition,kickoffPosition, _ball, ownGoal, targetGoal, playerData, country);
			playerNodes.Add(player);
			AddChild(player);
		}
		return playerNodes;
	}

	private Player SpawnPlayer(Vector2 playerPosition,Vector2 kickoffPosition, Ball ball, Goal ownGoal, Goal targetGoal, PlayerResource playerData, string country)
	{
		Player player = (Player)PLAYER_PREFAB.Instantiate();
		player.Init(playerPosition, kickoffPosition,ball, ownGoal, targetGoal, playerData, country);
		player.OnSwapRequest += OnPlayerSwapRequest;
		return player;
	}



	private void SetOnDutyWeights()
	{
		var squads = _squadHome.Concat(_squadAway);
		List<Player> cpuPlayers = squads.FilterCpuAndNoGoalkeeper().ToList();
		cpuPlayers.SortByDistanceTo(_ball.GlobalPosition);

		for (int i = 0; i < cpuPlayers.Count; i++)
		{
			cpuPlayers[i]._WeightOnDutySteering = 1 - Mathf.Ease((float)i / 30.0f, 0.1f);
		}

	}

	private void OnPlayerSwapRequest(Player requester)
	{
		var squad = requester._Country == _squadHome[0]._Country ? _squadHome : _squadAway;
		List<Player> cpuPlayers = squad.FilterCpuAndNoGoalkeeper().ToList();
		cpuPlayers.SortByDistanceTo(_ball.GlobalPosition);
		var closestCpuToBall = cpuPlayers[0];
		if (closestCpuToBall.Position.DistanceSquaredTo(_ball.Position) < requester.Position.DistanceSquaredTo(_ball.Position))
		{
			var playerControlScheme = requester._controlScheme;
			requester.SetControlScheme(Player.ControlScheme.CPU);
			closestCpuToBall.SetControlScheme(playerControlScheme);;

		}
	}

	private void SetupControlSchemes()
	{
		ResetControlSchemes();
		var p1Country = GameManager.Instance._PlayerSetup[0];
		if (GameManager.Instance.IsCoop())
		{
			var playerSquad = _squadHome[0]._Country == p1Country ? _squadHome : _squadAway;
			playerSquad[4].SetControlScheme(Player.ControlScheme.P1);
			playerSquad[5].SetControlScheme(Player.ControlScheme.P2);
		}
		else if (GameManager.Instance.IsSinglePlayer())
		{
			var playerSquad = _squadHome[0]._Country == p1Country ? _squadHome : _squadAway;
			playerSquad[5].SetControlScheme(Player.ControlScheme.P1);
		}
		else
		{
			var p1Squad = _squadHome[0]._Country == p1Country ? _squadHome : _squadAway;
			var p2Squad = p1Squad == _squadAway ? _squadHome : _squadAway;
			p1Squad[5].SetControlScheme(Player.ControlScheme.P1);
			p2Squad[5].SetControlScheme(Player.ControlScheme.P2);
		}
	}

	private void ResetControlSchemes()
	{
		var squads = _squadHome.Concat(_squadAway);
		foreach (Player player in squads)
		{
			player.SetControlScheme(Player.ControlScheme.CPU);
		}
	}

	private void CheckingForKickoffReadiness()
	{
		var squads = _squadHome.Concat(_squadAway);
		foreach (Player squad in squads)
		{
			if (!squad.IsReadyForKickoff())
			{
				return;
			}
		}
		SetupControlSchemes();
		_isCheckingForKickoffReadiness = false;
		SignalManager.EmitOnKickoffReady();
	}
	
	private void OnTeamReset()
	{
		_isCheckingForKickoffReadiness = true;
	}



}
