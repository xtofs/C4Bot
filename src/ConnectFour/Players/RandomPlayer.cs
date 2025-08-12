namespace ConnectFour.Players;

/// <summary>
/// A player that selects moves randomly from available columns.
/// </summary>
public class RandomPlayer : IPlayer
{
    /// <inheritdoc/>
    public string PlayerName { get; }

    public string AlgorithmName => "Random";

    private readonly Random _random;

    /// <summary>
    /// Creates a new random player with the given name.
    /// </summary>
    /// <param name="name">The display name for the player.</param>
    public RandomPlayer(string name)
    {
        PlayerName = name;
        _random = new Random();
    }

    /// <inheritdoc/>
    public int ChooseMove(GameBoard board, CellState player)
    {
        Span<int> buffer = stackalloc int[GameBoard.Columns];
        var moves = board.GetAvailableMoves(buffer);
        
        return moves[_random.Next(moves.Length)];
    }
}
