namespace ConnectFour.Players;

/// <summary>
/// A Negamax player with enhanced position evaluation using Connect Four-specific heuristics.
/// Uses the ConnectFourPositionEvaluator if no custom evaluator is provided.

/// </summary>
/// <remarks>
/// Creates a Negamax player with heuristics.
/// </remarks>
/// <param name="name">The display name for the player.</param>
/// <param name="maxDepth">The maximum search depth (default 6).</param>
public class NegamaxWithHeuristicPlayer(string name, int maxDepth, IPositionEvaluator evaluator) : IPlayer
{
    public NegamaxWithHeuristicPlayer(string name, int maxDepth) : this (name, maxDepth, new ConnectFourPositionEvaluator())
    {
    }

    private readonly NegamaxEngine engine = new NegamaxEngine(evaluator, maxDepth);

    /// <inheritdoc/>
    public string PlayerName { get; } = name ?? throw new ArgumentNullException(nameof(name));

    /// <inheritdoc/>
    public string AlgorithmName => $"Negamax enhanced ({engine.MaxDepth}, Heuristic1)";

    /// <inheritdoc/>
    public int ChooseMove(GameBoard board, CellState player)
    {
        return engine.FindBestMove(board, player);
    }
}
