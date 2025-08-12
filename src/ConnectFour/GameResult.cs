namespace ConnectFour;

/// <summary>
/// Represents only terminal game results (no ongoing state).
/// Values of GameResult can be compared and converted to GameState.
/// </summary>
public readonly struct GameResult : IEquatable<GameResult>, IEquatable<GameState>
{
    public enum Values : byte { XWin = 1, OWin = 2, Draw = 3 }

    public Values Value { get; }

    private GameResult(Values value) => Value = value;

    public static readonly GameResult XWin = new GameResult(Values.XWin);
    public static readonly GameResult OWin = new GameResult(Values.OWin);
    public static readonly GameResult Draw = new GameResult(Values.Draw);

    // Legacy compatibility - map to old enum names
    public static readonly GameResult WinX = XWin;
    public static readonly GameResult WinO = OWin;

    public override string ToString() => Value switch
    {
        Values.XWin => "XWin",
        Values.OWin => "OWin",
        Values.Draw => "Draw",
        _ => throw new ArgumentOutOfRangeException(nameof(Value), "Invalid GameResult value")
    };

    public bool Equals(GameResult other) => Value == other.Value;

    public bool Equals(GameState other)
    {
        // Works because of carefully crafted enums: GameState 0,1,2,3 and GameResult 1,2,3
        return other.Value != GameState.Values.Ongoing && (byte)Value == (byte)other.Value;
    }

    /// <summary>
    /// Implicitly converts a GameState to a GameResult (throws if Ongoing).
    /// </summary>
    public static implicit operator GameResult(GameState state) => state.Value switch
    {
        GameState.Values.XWin => GameResult.XWin,
        GameState.Values.OWin => GameResult.OWin,
        GameState.Values.Draw => GameResult.Draw,
        GameState.Values.Ongoing => throw new ArgumentException("Cannot convert Ongoing GameState to GameResult"),
        _ => throw new ArgumentOutOfRangeException(nameof(state), state, "Invalid GameState value")
    };

    public override bool Equals(object? obj)
    {
        return (obj is GameResult gameResult && Equals(gameResult)) || 
               (obj is GameState gameState && Equals(gameState));
    }

    public static bool operator ==(GameResult left, GameResult right) => left.Value == right.Value;
    public static bool operator !=(GameResult left, GameResult right) => !(left == right);

    public override int GetHashCode() => Value.GetHashCode();

    /// <summary>
    /// Gets the winning player from this game result (null for Draw).
    /// </summary>
    public Player? GetWinningPlayer() => Value switch
    {
        Values.XWin => Player.X,
        Values.OWin => Player.O,
        Values.Draw => null,
        _ => throw new ArgumentOutOfRangeException(nameof(Value), "Invalid GameResult value")
    };
}
