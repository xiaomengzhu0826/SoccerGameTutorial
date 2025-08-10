using Godot;
using System;

public partial class ActorsContainer : Node2D
{
	private static readonly PackedScene PLAYER_PREFAB = GD.Load<PackedScene>("res://Scenes/Characters/Player.tscn");

	[Export] private Ball _ball;
	[Export] private Goal _goalHome;
	[Export] private Goal _goalAway;
	[Export] private string _teamHome;
	[Export] private string _teamAway;

	private Node2D _spawns;

	public override void _Ready()
	{
		_spawns = GetNode<Node2D>("Spawns");
		SpawnPlayer(_teamHome, _goalHome);
		_spawns.Scale = new Vector2(-1, 1);
		SpawnPlayer(_teamAway, _goalAway);

		foreach (var item in GetChildren())
		{
			if (item is Player player && item.GetIndex() == 8)
			{
				player._controlScheme = Player.ControlScheme.P1;
				player.SetControlTexture();
			}
		}
	}

	private void SpawnPlayer(string country, Goal ownGoal)
	{
		var players = DataLoader.Instance.GetSquad(country);
		Goal targetGoal = (_goalHome == ownGoal) ? _goalAway : _goalHome;
		for (int i = 0; i < players.Count; i++)
		{
			Node2D child = (Node2D)_spawns.GetChild(i);
			var playerPosition = child.GlobalPosition;
			PlayerResource playerData = players[i];
			var player = SpawnPlayer(playerPosition, _ball, ownGoal, targetGoal, playerData,country);
			AddChild(player);
		}
	}

	private Player SpawnPlayer(Vector2 playerPosition, Ball ball, Goal ownGoal, Goal targetGoal, PlayerResource playerData,string country)
	{
		Player player = (Player)PLAYER_PREFAB.Instantiate();
		player.Init(playerPosition, ball, ownGoal, targetGoal, playerData,country);
		return player;
	}

}
