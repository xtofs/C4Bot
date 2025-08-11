namespace ConnectFour;

/// <summary>
/// Represents a Connect Four player (X or O only, never Empty).
/// </summary>
public enum Player : byte
{
    X = 1,
    O = 2
}

/// <summary>
/// Represents the state of a cell on the Connect Four board.
/// </summary>
public enum CellState : byte
{
    Empty = 0,
    X = 1,
    O = 2
}

/// <summary>
/// Extension methods for Player and CellState conversions.
/// </summary>
public static class CellStateExtensions
{
    /// <summary>
    /// Converts a Player to its corresponding CellState.
    /// </summary>
    public static CellState ToCellState(this Player player) => player switch
    {
        Player.X => CellState.X,
        Player.O => CellState.O,
        _ => throw new ArgumentOutOfRangeException(nameof(player))
    };

    /// <summary>
    /// Converts a CellState to a Player (throws if Empty).
    /// </summary>
    public static Player ToPlayer(this CellState cellState) => cellState switch
    {
        CellState.X => Player.X,
        CellState.O => Player.O,
        CellState.Empty => throw new ArgumentException("Cannot convert Empty CellState to Player"),
        _ => throw new ArgumentOutOfRangeException(nameof(cellState))
    };

    /// <summary>
    /// Safely converts a CellState to a Player, returning null for Empty.
    /// </summary>
    public static Player? ToPlayerOrNull(this CellState cellState) => cellState switch
    {
        CellState.X => Player.X,
        CellState.O => Player.O,
        CellState.Empty => null,
        _ => throw new ArgumentOutOfRangeException(nameof(cellState))
    };

    /// <summary>
    /// Gets the opponent of a player.
    /// </summary>
    public static Player Opponent(this Player player) => player switch
    {
        Player.X => Player.O,
        Player.O => Player.X,
        _ => throw new ArgumentOutOfRangeException(nameof(player))
    };
}