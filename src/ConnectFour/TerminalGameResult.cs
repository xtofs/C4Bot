namespace ConnectFour;

/// <summary>
/// Represents only terminal game results (no ongoing state).
/// Use nullable TerminalGameResult when you need to express "no result yet".
/// </summary>
public enum TerminalGameResult : byte
{
    WinX = 1,
    WinO = 2,
    Draw = 3
}
