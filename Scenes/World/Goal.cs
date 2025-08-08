using Godot;
using System;

public partial class Goal : Node2D
{
	private Area2D _backNetArea;
	private Node2D _targets;

	public override void _Ready()
	{
		_backNetArea = GetNode<Area2D>("BackNetArea");
		_targets = GetNode<Node2D>("Targets");
		_backNetArea.BodyEntered += OnBallEnterNet;
	}

	private void OnBallEnterNet(Node2D body)
	{
		Ball ball = body as Ball;
		ball.Stop();
	}

	public Vector2 GetRandomTargetPosition()
	{
		var child = (Node2D)_targets.GetChild(GD.RandRange(0, _targets.GetChildCount()-1));
		return child.GlobalPosition;
	}


}
