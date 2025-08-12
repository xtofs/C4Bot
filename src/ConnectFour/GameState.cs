namespace ConnectFour;

/// <summary>
/// Represents all possible states of a Connect Four game (including ongoing).
/// Values of GameState can be compared and converted to GameResult (with limitations).
/// </summary>
public readonly struct GameState : IEquatable<GameState>, IEquatable<GameResult>
{
    public enum Values : byte { Ongoing = 0, XWin = 1, OWin = 2, Draw = 3 }

    public Values Value { get; }

    private GameState(Values value) => Value = value;

    public static readonly GameState Ongoing = new GameState(Values.Ongoing);
    public static readonly GameState XWin = new GameState(Values.XWin);
    public static readonly GameState OWin = new GameState(Values.OWin);
    public static readonly GameState Draw = new GameState(Values.Draw);

    // Legacy compatibility - map to old enum names
    public static readonly GameState WinX = XWin;
    public static readonly GameState WinO = OWin;

    public override string ToString() => Value switch
    {
        Values.Ongoing => "Ongoing",
        Values.XWin => "XWin",
        Values.OWin => "OWin", 
        Values.Draw => "Draw",
        _ => throw new ArgumentOutOfRangeException(nameof(Value), "Invalid GameState value")
    };

    public bool Equals(GameState other) => Value == other.Value;

    public bool Equals(GameResult other)
    {
        // Works because of carefully crafted enums: GameState 0,1,2,3 and GameResult 1,2,3
        return Value != Values.Ongoing && (byte)Value == (byte)other.Value;
    }

    /// <summary>
    /// Implicitly converts a GameResult to a GameState.
    /// </summary>
    public static implicit operator GameState(GameResult result) => result.Value switch
    {
        GameResult.Values.XWin => GameState.XWin,
        GameResult.Values.OWin => GameState.OWin,
        GameResult.Values.Draw => GameState.Draw,
        _ => throw new ArgumentOutOfRangeException(nameof(result), result, "Invalid GameResult value")
    };

    /// <summary>
    /// Implicitly converts a GameState to a GameResult (returns null for Ongoing).
    /// </summary>
    public static implicit operator GameResult?(GameState state) => state.Value switch
    {
        Values.Ongoing => null,
        Values.XWin => GameResult.XWin,
        Values.OWin => GameResult.OWin,
        Values.Draw => GameResult.Draw,
        _ => throw new ArgumentOutOfRangeException(nameof(state), state, "Invalid GameState value")
    };

    public override bool Equals(object? obj)
    {
        return (obj is GameState gameState && Equals(gameState)) || 
               (obj is GameResult gameResult && Equals(gameResult));
    }

    public static bool operator ==(GameState left, GameState right) => left.Value == right.Value;
    public static bool operator !=(GameState left, GameState right) => !(left == right);

    public override int GetHashCode() => Value.GetHashCode();

    /// <summary>
    /// Gets the winning player from this game state (null for Draw or Ongoing).
    /// </summary>
    public Player? GetWinningPlayer() => Value switch
    {
        Values.XWin => Player.X,
        Values.OWin => Player.O,
        Values.Draw => null,
        Values.Ongoing => null,
        _ => throw new ArgumentOutOfRangeException(nameof(Value), "Invalid GameState value")
    };

    /// <summary>
    /// Returns true if the game has ended (not ongoing).
    /// </summary>
    public bool IsTerminal => Value != Values.Ongoing;
}
