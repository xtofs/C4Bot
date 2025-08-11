namespace ConnectFour;

/// <summary>
/// Extension methods for type-safe conversions between Player, CellState, and result enums.
/// </summary>
public static class EnumExtensions
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

    /// <summary>
    /// Converts a TerminalGameResult to the legacy GameResult.
    /// </summary>
    public static GameResult ToGameResult(this TerminalGameResult terminalResult) => terminalResult switch
    {
        TerminalGameResult.WinX => GameResult.WinX,
        TerminalGameResult.WinO => GameResult.WinO,
        TerminalGameResult.Draw => GameResult.Draw,
        _ => throw new ArgumentOutOfRangeException(nameof(terminalResult))
    };

    /// <summary>
    /// Converts a GameResult to TerminalGameResult (throws if Ongoing).
    /// </summary>
    public static TerminalGameResult ToTerminalResult(this GameResult gameResult) => gameResult switch
    {
        GameResult.WinX => TerminalGameResult.WinX,
        GameResult.WinO => TerminalGameResult.WinO,
        GameResult.Draw => TerminalGameResult.Draw,
        GameResult.Ongoing => throw new ArgumentException("Cannot convert Ongoing GameResult to TerminalGameResult"),
        _ => throw new ArgumentOutOfRangeException(nameof(gameResult))
    };

    /// <summary>
    /// Gets the winning player from a terminal result (null for Draw).
    /// </summary>
    public static Player? GetWinningPlayer(this TerminalGameResult result) => result switch
    {
        TerminalGameResult.WinX => Player.X,
        TerminalGameResult.WinO => Player.O,
        TerminalGameResult.Draw => null,
        _ => throw new ArgumentOutOfRangeException(nameof(result))
    };
}
