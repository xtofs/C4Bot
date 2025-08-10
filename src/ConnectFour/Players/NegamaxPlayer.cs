namespace ConnectFour.Players;

/// <summary>
/// A Negamax player with basic evaluation (terminal positions only).
/// Uses the NegamaxEngine with a BasicPositionEvaluator.
/// </summary>
/// <remarks>
/// Creates a new Negamax player with basic evaluation.
/// </remarks>
/// <param name="name">The display name for the player.</param>
/// <param name="maxDepth">The maximum search depth (default 6).</param>
public class NegamaxPlayer(string name, int maxDepth) : IPlayer
{
    private readonly NegamaxEngine engine = new NegamaxEngine(new BasicPositionEvaluator(), maxDepth);

    /// <inheritdoc/>
    public string PlayerName { get; } = name ?? throw new ArgumentNullException(nameof(name));

    /// <inheritdoc/>
    public string AlgorithmName => $"Negamax ({engine.MaxDepth})";

    /// <inheritdoc/>
    public int ChooseMove(GameBoard board, CellState player)
    {
        return engine.FindBestMove(board, player);
    }
}
