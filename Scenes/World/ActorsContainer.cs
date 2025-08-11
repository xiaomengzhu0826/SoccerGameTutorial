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
	[Export] private string _teamHome;
	[Export] private string _teamAway;

	private Node2D _spawns;

	private List<Player> _squadHome=new();
	private List<Player> _squadAway=new();
	private float _timeSinceLastCacheRefresh = Time.GetTicksMsec();
	private List<Player> cpuPlayers=new();

	public override void _Ready()
	{
		_spawns = GetNode<Node2D>("Spawns");
		_squadHome = SpawnPlayer(_teamHome, _goalHome);
		_spawns.Scale = new Vector2(-1, 1);
		_squadAway = SpawnPlayer(_teamAway, _goalAway);

		foreach (var item in GetChildren())
		{
			if (item is Player player && item.GetIndex() == 8)
			{
				player._controlScheme = Player.ControlScheme.P1;
				player.SetControlTexture();
			}
		}
	}

	public override void _Process(double delta)
	{
		if (Time.GetTicksMsec() - _timeSinceLastCacheRefresh > DURATION_WEIGHT_CACHE)
		{
			_timeSinceLastCacheRefresh = Time.GetTicksMsec();
			SetOnDutyWeights();
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
			var player = SpawnPlayer(playerPosition, _ball, ownGoal, targetGoal, playerData, country);
			playerNodes.Add(player);
			AddChild(player);
		}
		return playerNodes;
	}

	private Player SpawnPlayer(Vector2 playerPosition, Ball ball, Goal ownGoal, Goal targetGoal, PlayerResource playerData, string country)
	{
		Player player = (Player)PLAYER_PREFAB.Instantiate();
		player.Init(playerPosition, ball, ownGoal, targetGoal, playerData, country);
		return player;
	}

	private void SetOnDutyWeights()
	{
		cpuPlayers.Clear();
		var squads = _squadHome.Concat(_squadAway);
		cpuPlayers = squads.FilterCpuAndNoGoalkeeper().ToList();

		cpuPlayers.SortByDistanceTo(_ball.GlobalPosition);

		for (int i = 0; i < cpuPlayers.Count; i++)
		{
			cpuPlayers[i]._weightOnDutySteering = 1 - Mathf.Ease((float)i / 30.0f, 0.1f);
		}

	}



}
