namespace ConnectFour.Players;

/// <summary>
/// A Minimax player with basic evaluation (terminal positions only).
/// Uses the MinimaxEngine with a BasicPositionEvaluator.
/// </summary>
/// <remarks>
/// Creates a new Minimax player with basic evaluation.
/// </remarks>
/// <param name="name">The display name for the player.</param>
/// <param name="maxDepth">The maximum search depth (default 6).</param>
public class MinimaxPlayer(string name, int maxDepth) : IPlayer
{
    private readonly MinimaxEngine engine = new MinimaxEngine(new BasicPositionEvaluator(), maxDepth);

    /// <inheritdoc/>
    public string PlayerName { get; } = name ?? throw new ArgumentNullException(nameof(name));

    /// <inheritdoc/>
    public string AlgorithmName => $"Minimax ({engine.MaxDepth})";

    /// <inheritdoc/>
    public int ChooseMove(GameBoard board, CellState player)
    {
        return engine.FindBestMove(board, player);
    }
}
