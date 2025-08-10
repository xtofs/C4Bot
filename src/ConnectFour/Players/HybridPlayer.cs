using System;

namespace ConnectFour;

/// <summary>
/// A hybrid player that uses different algorithms for different game phases.
/// Uses MCTS for opening (positional understanding) and Enhanced Negamax for endgame (tactical precision).
/// </summary>
public class HybridPlayer : IPlayer
{
    private readonly IPlayer openingPlayer;   // MCTS for positional play
    private readonly IPlayer endgamePlayer;   // Enhanced Negamax for tactical precision
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
    /// <param name="negamaxDepth">Negamax search depth for endgame play (default 6).</param>
    /// <param name="phaseTransition">Move number to transition from opening to endgame (default 14).</param>
    public HybridPlayer(string playerName, int mctsSimulations = 1000, int negamaxDepth = 6, int phaseTransition = 14)
    {
        PlayerName = playerName ?? throw new ArgumentNullException(nameof(playerName));
        AlgorithmName = $"Hybrid(MCTS-{mctsSimulations}/Enhanced-{negamaxDepth})";
        
        openingPlayer = new MonteCarloTreeSearchPlayer("Opening-MCTS", mctsSimulations);
        endgamePlayer = new EnhancedNegamaxPlayer("Endgame-Enhanced", negamaxDepth);
        phaseTransitionMove = phaseTransition;
    }

    /// <inheritdoc/>
    public int ChooseMove(GameBoard board, CellState player)
    {
        int moveCount = CountTotalMoves(board);
        
        // Opening phase: Use MCTS for positional understanding
        if (moveCount < phaseTransitionMove)
        {
            return openingPlayer.ChooseMove(board, player);
        }
        
        // Endgame phase: Use Enhanced Negamax for tactical precision
        return endgamePlayer.ChooseMove(board, player);
    }

    /// <summary>
    /// Counts the total number of moves played on the board.
    /// </summary>
    private static int CountTotalMoves(GameBoard board)
    {
        int count = 0;
        for (int row = 0; row < GameBoard.Rows; row++)
        {
            for (int col = 0; col < GameBoard.Columns; col++)
            {
                if (board[row, col] != CellState.Empty)
                    count++;
            }
        }
        return count;
    }
}
