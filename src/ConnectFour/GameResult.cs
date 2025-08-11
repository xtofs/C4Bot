namespace ConnectFour;

/// <summary>
/// Represents all possible states of a Connect Four game (including ongoing).
/// </summary>
public enum GameState 
{ 
    Ongoing, 
    WinX, 
    WinO, 
    Draw 
}

/// <summary>
/// Represents only terminal game results (no ongoing state).
/// Use nullable GameResult when you need to express "no result yet".
/// </summary>
public enum GameResult : byte
{
    WinX = 1,
    WinO = 2,
    Draw = 3
}

/// <summary>
/// Extension methods for GameState and GameResult conversions.
/// </summary>
public static class GameStateExtensions
{
    /// <summary>
    /// Converts a GameResult to the legacy GameState.
    /// </summary>
    public static GameState ToGameState(this GameResult gameResult) => gameResult switch
    {
        GameResult.WinX => GameState.WinX,
        GameResult.WinO => GameState.WinO,
        GameResult.Draw => GameState.Draw,
        _ => throw new ArgumentOutOfRangeException(nameof(gameResult))
    };

    /// <summary>
    /// Converts a GameState to GameResult (throws if Ongoing).
    /// </summary>
    public static GameResult ToGameResult(this GameState gameState) => gameState switch
    {
        GameState.WinX => GameResult.WinX,
        GameState.WinO => GameResult.WinO,
        GameState.Draw => GameResult.Draw,
        GameState.Ongoing => throw new ArgumentException("Cannot convert Ongoing GameState to GameResult"),
        _ => throw new ArgumentOutOfRangeException(nameof(gameState))
    };

    /// <summary>
    /// Safely converts a GameState to GameResult, returning null for Ongoing.
    /// </summary>
    public static GameResult? ToGameResultOrNull(this GameState gameState) => gameState switch
    {
        GameState.WinX => GameResult.WinX,
        GameState.WinO => GameResult.WinO,
        GameState.Draw => GameResult.Draw,
        GameState.Ongoing => null,
        _ => throw new ArgumentOutOfRangeException(nameof(gameState))
    };

    /// <summary>
    /// Gets the winning player from a game result (null for Draw).
    /// </summary>
    public static Player? GetWinningPlayer(this GameResult result) => result switch
    {
        GameResult.WinX => Player.X,
        GameResult.WinO => Player.O,
        GameResult.Draw => null,
        _ => throw new ArgumentOutOfRangeException(nameof(result))
    };

    /// <summary>
    /// Gets the winning player from a game state (null for Draw or Ongoing).
    /// </summary>
    public static Player? GetWinningPlayer(this GameState state) => state switch
    {
        GameState.WinX => Player.X,
        GameState.WinO => Player.O,
        GameState.Draw => null,
        GameState.Ongoing => null,
        _ => throw new ArgumentOutOfRangeException(nameof(state))
    };
}
