using Godot;
using System;


public partial class Ui : CanvasLayer
{
	private Label _playerLabel;
	private TextureRect _homeFlagTexture;
	private Label _scoreLabel;
	private TextureRect _awayFlagTexture;
	private Label _timeLabel;
	private AnimationPlayer _animationPlayer;
	private Label _goalScorerLabel;
	private Label _scoreInfoLabel;

	private TextureRect[] _flagTextures;
	private string _lastBallCarrier;

	public override void _Ready()
	{
		_playerLabel = GetNode<Label>("%PlayerLabel");
		_homeFlagTexture = GetNode<TextureRect>("%HomeFlagTexture");
		_scoreLabel = GetNode<Label>("%ScoreLabel");
		_awayFlagTexture = GetNode<TextureRect>("%AwayFlagTexture");
		_timeLabel = GetNode<Label>("%TimeLabel");
		_animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
		_goalScorerLabel = GetNode<Label>("%GoalScorerLabel");
		_scoreInfoLabel = GetNode<Label>("%ScoreInfoLabel");

		_flagTextures = new[] { _homeFlagTexture, _awayFlagTexture };

		SignalManager.Instance.OnBallPossessed += OnBallPossessed;
		SignalManager.Instance.OnBallReleased += OnBallReleased;
		SignalManager.Instance.OnScoreChanged += OnScoreChanged;
		SignalManager.Instance.OnTeamReset += OnTeamReset;
		SignalManager.Instance.OnGameOver += OnGameOver;

		UpdateScore();
		UpdateFlag();
		UpdateClock();
		_playerLabel.Text = "";
	}

	public override void _ExitTree()
	{
		SignalManager.Instance.OnBallPossessed -= OnBallPossessed;
		SignalManager.Instance.OnBallReleased -= OnBallReleased;
		SignalManager.Instance.OnScoreChanged += OnScoreChanged;
		SignalManager.Instance.OnTeamReset -= OnTeamReset;
		SignalManager.Instance.OnGameOver -= OnGameOver;
	}

	public override void _Process(double delta)
	{
		UpdateClock();
	}

	public void UpdateScore()
	{
		_scoreLabel.Text = ToolUtils.GetScoreText(GameManager.Instance._Score);
	}

	public void UpdateFlag()
	{
		for (int i = 0; i < _flagTextures.Length; i++)
		{
			_flagTextures[i].Texture = ToolUtils.GetTexture(GameManager.Instance._Countries[i]);
		}
	}

	public void UpdateClock()
	{
		if (GameManager.Instance._TimeLeft < 0)
		{
			_timeLabel.Modulate = Colors.Yellow;
		}
		_timeLabel.Text = ToolUtils.GetTimeText(GameManager.Instance._TimeLeft);
	}

	private void OnBallPossessed(string name)
	{
		_playerLabel.Text = name;
		_lastBallCarrier = name;
	}

	private void OnBallReleased()
	{
		_playerLabel.Text = string.Empty;
	}

	private void OnScoreChanged()
	{
		if (!GameManager.Instance.IsTimeUp())
		{
			_goalScorerLabel.Text = $"{_lastBallCarrier} SCORED!";
			_scoreInfoLabel.Text = ToolUtils.GetCurrentScoreInfo(GameManager.Instance._Countries, GameManager.Instance._Score);
			_animationPlayer.Play("goal_appear");
		}

		UpdateScore();
	}

	private void OnTeamReset()
	{
		if (GameManager.Instance.HasSomeoneScored())
		{
			_animationPlayer.Play("goal_hide");
		}
		
	}

	private void OnGameOver(string countryWinner)
	{
		_scoreInfoLabel.Text = ToolUtils.GetFinalScoreInfo(GameManager.Instance._Countries, GameManager.Instance._Score);
		_animationPlayer.Play("game_over");
	}
}
