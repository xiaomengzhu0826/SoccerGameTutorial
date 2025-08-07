using Godot;
using System;

public partial class Goal : Node2D
{
	private Area2D _backNetArea;

	public override void _Ready()
	{
		_backNetArea = GetNode<Area2D>("BackNetArea");
		_backNetArea.BodyEntered += OnBallEnterNet;
	}

	private void OnBallEnterNet(Node2D body)
	{
		Ball ball = body as Ball;
		ball.Stop();
    }

}
