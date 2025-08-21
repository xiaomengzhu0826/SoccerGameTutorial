using Godot;
using System;

public partial class MainMenuScreen : Control
{
	private static readonly Texture2D[][] MENU_TEXTURES =
	[
		[GD.Load<Texture2D>("res://Assets/art/ui/mainmenu/1-player.png"),GD.Load<Texture2D>("res://Assets/art/ui/mainmenu/1-player-selected.png")],
		[GD.Load<Texture2D>("res://Assets/art/ui/mainmenu/2-players.png"),GD.Load<Texture2D>("res://Assets/art/ui/mainmenu/2-players-selected.png")]
	];

	private TextureRect _singlePlayerTexture;
	private TextureRect _twoPlayersTexture;
	private TextureRect _selectionTexture;

	private int _currecntSelectedIndex;
	private TextureRect[] _selectableMenuNodes;
	private bool _isActive;

	public override void _Ready()
	{
		_singlePlayerTexture = GetNode<TextureRect>("%SinglePlayerTexture");
		_twoPlayersTexture = GetNode<TextureRect>("%TwoPlayersTexture");
		_selectionTexture = GetNode<TextureRect>("%SelectionTexture");

		_selectableMenuNodes = [_singlePlayerTexture, _twoPlayersTexture];
		RefreshUi();
	}

	public override void _Process(double delta)
	{
		if (_isActive)
		{
			if (KeyUtils.IsActionJustPressed(Player.ControlScheme.P1, KeyUtils.Action.UP))
			{
				ChangeSelectedIndex(_currecntSelectedIndex - 1);
			}
			else if (KeyUtils.IsActionJustPressed(Player.ControlScheme.P1, KeyUtils.Action.DOWN))
			{
				ChangeSelectedIndex(_currecntSelectedIndex + 1);
			}
			else if (KeyUtils.IsActionJustPressed(Player.ControlScheme.P1, KeyUtils.Action.SHOOT))
			{
				SubmitSelection();
			}
		}

	}

	private void RefreshUi()
	{
		for (int i = 0; i < _selectableMenuNodes.Length; i++)
		{
			if (_currecntSelectedIndex == i)
			{
				_selectableMenuNodes[i].Texture = MENU_TEXTURES[i][1];
				_selectionTexture.Position = _selectableMenuNodes[i].Position + Vector2.Left * 25;
			}
			else
			{
				_selectableMenuNodes[i].Texture = MENU_TEXTURES[i][0];
			}
		}
	}

	private void SubmitSelection()
	{
		SoundManager.Instance.Play(SoundManager.Sound.UI_SELECT);
		var countryDefault = DataLoader.Instance._Countries[0];
		string playerTwo = _currecntSelectedIndex == 0 ? "" : countryDefault;
		GameManager.Instance._PlayerSetup = [countryDefault, playerTwo];
	}

	private void ChangeSelectedIndex(int newIndex)
	{
		_currecntSelectedIndex = Mathf.Clamp(newIndex, 0, _selectableMenuNodes.Length - 1);
		SoundManager.Instance.Play(SoundManager.Sound.UI_NAV);
		RefreshUi();
	}

	public void OnSetActive()
	{
		RefreshUi();
		_isActive = true;
	}
}
