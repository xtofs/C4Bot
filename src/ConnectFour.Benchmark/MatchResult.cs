namespace ConnectFour.Benchmark;

/// <summary>
/// Represents the result of a tournament match between two players.
/// </summary>
public record MatchResult(
    string Player1Name,
    string Player2Name,
    int Player1Wins,
    int Player2Wins,
    int Draws,
    int TotalGames,
    TimeSpan TotalTime)
{
    public double Player1WinRate => TotalGames > 0 ? (double)Player1Wins / TotalGames : 0.0;
    public double Player2WinRate => TotalGames > 0 ? (double)Player2Wins / TotalGames : 0.0;
    public double DrawRate => TotalGames > 0 ? (double)Draws / TotalGames : 0.0;
    public TimeSpan AverageGameTime => TotalGames > 0 ? TimeSpan.FromMilliseconds(TotalTime.TotalMilliseconds / TotalGames) : TimeSpan.Zero;
}
