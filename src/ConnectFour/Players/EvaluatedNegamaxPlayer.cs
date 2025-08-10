using System;

namespace ConnectFour;

/// <summary>
/// A Negamax player with enhanced position evaluation using Connect Four-specific heuristics.
/// Uses the NegamaxEngine with a ConnectFourPositionEvaluator.
/// </summary>
public class EnhancedNegamaxPlayer : IPlayer
{
    private readonly NegamaxEngine algorithm;

    /// <inheritdoc/>
    public string Name { get; }

    /// <summary>
    /// Creates a new Enhanced Negamax player.
    /// </summary>
    /// <param name="name">The display name for the player.</param>
    /// <param name="maxDepth">The maximum search depth (default 6).</param>
    public EnhancedNegamaxPlayer(string name, int maxDepth = 6)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        algorithm = new NegamaxEngine(new ConnectFourPositionEvaluator(), maxDepth);
    }

    /// <inheritdoc/>
    public int ChooseMove(GameBoard board, CellState player)
    {
        return algorithm.FindBestMove(board, player);
    }
}
