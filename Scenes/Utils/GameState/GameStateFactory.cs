using Godot;
using System;
using System.Collections.Generic;

public class GameStateFactory
{
    public Dictionary<GameManager.State,  Func<GameState>> _States;

    public GameStateFactory()
    {
        _States = new()
        {
            {GameManager.State.IN_PLAY, () => new GameStateInPlay() },
            {GameManager.State.GAMEOVER, () => new GameStateGameOver() },
            {GameManager.State.OVERTIME, () => new GameStateOverTime() },
            {GameManager.State.RESET, () => new GameStateReset() },
            {GameManager.State.SCORED, () => new GameStateScored() },
            {GameManager.State.KICKOFF, () => new GameStateKickOff() },
        };
    }

    public GameState GetFreshState(GameManager.State state)
    {
        if (!_States.TryGetValue(state, out Func<GameState> value))
        {
            throw new Exception("State does not exist");
        }
        return value();
    }
}
