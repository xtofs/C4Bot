using System;

namespace ConnectFour;

/// <summary>
/// A Negamax player with enhanced position evaluation using Connect Four-specific heuristics.
/// Uses the NegamaxEngine with a ConnectFourPositionEvaluator.
/// The heuristics provide:
///   Center column preference (columns 3-4 most valuable)
///   Threat detection (3-in-a-row patterns worth 50 points)
///   Defensive prioritization (blocking threats worth 60 points)
///   Two-in-a-row building (foundation patterns worth 2 points)
/// </summary>
/// <remarks>
/// Creates a new Enhanced Negamax player.
/// </remarks>
/// <param name="name">The display name for the player.</param>
/// <param name="maxDepth">The maximum search depth (default 6).</param>
public class EnhancedNegamaxPlayer(string name, int maxDepth = 6) : IPlayer
{
    private readonly NegamaxEngine engine = new NegamaxEngine(new ConnectFourPositionEvaluator(), maxDepth);

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
