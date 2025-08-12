namespace ConnectFour;

/// <summary>
/// Extension methods for type-safe conversions between Player, CellState, and result enums.
/// </summary>
public static class EnumExtensions
{

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
    public static TerminalGameResult ToTerminalResult(this GameResult gameResult) => gameResult.Value switch
    {
        GameResult.Values.XWin => TerminalGameResult.WinX,
        GameResult.Values.OWin => TerminalGameResult.WinO,
        GameResult.Values.Draw => TerminalGameResult.Draw,
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
