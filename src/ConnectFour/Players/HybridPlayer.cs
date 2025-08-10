namespace ConnectFour.Players;

/// <summary>
/// A hybrid player that uses different algorithms for different game phases.
/// Uses MCTS for opening (positional understanding) and Enhanced Minimax for endgame (tactical precision).
/// </summary>
public class HybridPlayer : IPlayer
{
    private readonly IPlayer openingPlayer;   // MCTS for positional play
    private readonly IPlayer endgamePlayer;   // Enhanced Minimax for tactical precision
    private readonly int phaseTransitionMove;

    /// <inheritdoc/>
    public string PlayerName { get; }

    /// <inheritdoc/>
    public string AlgorithmName { get; }

    /// <summary>
    /// Creates a new Hybrid player.
    /// </summary>
    /// <param name="playerName">The display name for the player.</param>
    /// <param name="mctsSimulations">Number of MCTS simulations for opening play (default 1000).</param>
    /// <param name="minimaxDepth">Minimax search depth for endgame play (default 6).</param>
    /// <param name="phaseTransition">Move number to transition from opening to endgame (default 14).</param>
    public HybridPlayer(string playerName, int mctsSimulations = 1000, int minimaxDepth = 6, int phaseTransition = 14)
    {
        PlayerName = playerName ?? throw new ArgumentNullException(nameof(playerName));
        AlgorithmName = $"Hybrid(MCTS-{mctsSimulations}/Enhanced-{minimaxDepth})";
        
        openingPlayer = new MonteCarloTreeSearchPlayer("Opening-MCTS", mctsSimulations);
        endgamePlayer = new MinimaxWithHeuristicPlayer("Endgame-Enhanced", minimaxDepth);
        phaseTransitionMove = phaseTransition;
    }

    /// <inheritdoc/>
    public int ChooseMove(GameBoard board, CellState player)
    {
        int moveCount = board.HalfMoveCount;
        
        // Opening phase: Use MCTS for positional understanding
        if (moveCount < phaseTransitionMove)
        {
            return openingPlayer.ChooseMove(board, player);
        }
        
        // Endgame phase: Use Enhanced Minimax for tactical precision
        return endgamePlayer.ChooseMove(board, player);
    }
}
