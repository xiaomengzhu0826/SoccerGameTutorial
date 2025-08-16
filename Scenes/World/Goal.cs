using Godot;
using System;

public partial class Goal : Node2D
{
	private Area2D _backNetArea;
	private Node2D _targets;
	private Area2D _scoringArea;
	private string _country;

	public override void _Ready()
	{
		_backNetArea = GetNode<Area2D>("BackNetArea");
		_targets = GetNode<Node2D>("Targets");
		_scoringArea = GetNode<Area2D>("ScoringArea");

		_backNetArea.BodyEntered += OnBallEnterNet;
		_scoringArea.BodyEntered += OnBallEnterScoringArea;
	}

	public void Initialize(string contextCountry)
	{
		_country = contextCountry;
	}

    private void OnBallEnterNet(Node2D body)
	{
		Ball ball = body as Ball;
		ball.Stop();
	}

	private void OnBallEnterScoringArea(Node2D body)
    {
		SignalManager.EmitOnTeamScored(_country);
    }

	public Vector2 GetRandomTargetPosition()
	{
		var child = (Node2D)_targets.GetChild(GD.RandRange(0, _targets.GetChildCount() - 1));
		return child.GlobalPosition;
	}

	public Vector2 GetCenterTargetPosition()
	{
		var targets = (Node2D)_targets.GetChild(_targets.GetChildCount() / 2);
		return targets.GlobalPosition;
	}

	public Vector2 GetTopTargetPosition()
	{
		var targets = (Node2D)_targets.GetChild(0);
		return targets.GlobalPosition;
	}

	public Vector2 GetBottomTargetPosition()
	{
		var targets = (Node2D)_targets.GetChild(_targets.GetChildCount() - 1);
		return targets.GlobalPosition;
	}

	public Area2D GetScoringArea()
	{
		return _scoringArea;
	}




}
