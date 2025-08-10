using System;

namespace ConnectFour;

/// <summary>
/// A Negamax player with basic evaluation (terminal positions only).
/// Uses the NegamaxAlgorithm with a BasicPositionEvaluator.
/// </summary>
public class NegamaxPlayer : IPlayer
{
    private readonly NegamaxAlgorithm algorithm;

    /// <inheritdoc/>
    public string Name { get; }

    /// <summary>
    /// Creates a new Negamax player with basic evaluation.
    /// </summary>
    /// <param name="name">The display name for the player.</param>
    /// <param name="maxDepth">The maximum search depth (default 6).</param>
    public NegamaxPlayer(string name, int maxDepth = 6)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        algorithm = new NegamaxAlgorithm(new BasicPositionEvaluator(), maxDepth);
    }

    /// <inheritdoc/>
    public int ChooseMove(GameBoard board, CellState player)
    {
        return algorithm.FindBestMove(board, player);
    }
}
