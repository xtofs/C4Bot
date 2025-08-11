namespace ConnectFour.Players;

/// <summary>
/// The core Minimax algorithm with alpha-beta pruning, parameterized by search depth and position evaluator.
/// </summary>
public class MinimaxEngine
{
    private readonly IPositionEvaluator evaluator;

    public int MaxDepth { get; }

    /// <summary>
    /// Creates a new Minimax algorithm with the specified evaluator and search depth.
    /// </summary>
    /// <param name="evaluator">The position evaluator to use for leaf positions.</param>
    /// <param name="maxDepth">The maximum search depth.</param>
    public MinimaxEngine(IPositionEvaluator evaluator, int maxDepth)
    {
        this.evaluator = evaluator ?? throw new ArgumentNullException(nameof(evaluator));
        this.MaxDepth = maxDepth;
    }

    /// <summary>
    /// Finds the best move for the given player using Minimax search.
    /// </summary>
    /// <param name="board">The current game board.</param>
    /// <param name="player">The player to move.</param>
    /// <returns>The best move (column index).</returns>
    public int FindBestMove(GameBoard board, CellState player)
    {
        var bestMove = -1;
        var bestScore = int.MinValue;
        
        foreach (var move in board.GetAvailableMoves())
        {
            var newBoard = board.ApplyMove(move, player);
            var score = -Minimax(newBoard, Opponent(player), MaxDepth - 1, int.MinValue, int.MaxValue, player);
            if (score > bestScore)
            {
                bestScore = score;
                bestMove = move;
            }
        }
        
        return bestMove;
    }

    /// <summary>
    /// Minimax recursive search with alpha-beta pruning.
    /// </summary>
    private int Minimax(GameBoard board, CellState player, int depth, int alpha, int beta, CellState originalPlayer)
    {
        var result = board.GetGameState();
        switch (result)
        {
            case GameState.WinX:
                return player == CellState.X ? 10000 + depth : -10000 - depth;
            case GameState.WinO:
                return player == CellState.O ? 10000 + depth : -10000 - depth;
            case GameState.Draw:
                return 0;
            case GameState.Ongoing:
                // Continue to evaluation
                break;
        }

        // Use evaluator at leaf nodes
        if (depth == 0)
        {
            var eval = evaluator.Evaluate(board, originalPlayer);
            return player == originalPlayer ? eval : -eval;
        }

        var max = int.MinValue;
        foreach (var move in board.GetAvailableMoves())
        {
            var newBoard = board.ApplyMove(move, player);
            var score = -Minimax(newBoard, Opponent(player), depth - 1, -beta, -alpha, originalPlayer);
            if (score > max)
            {
                max = score;
            }

            if (max > alpha)
            {
                alpha = max;
            }

            if (alpha >= beta)
            {
                break; // Alpha-beta pruning
            }
        }
        return max;
    }

    private static CellState Opponent(CellState player) => player == CellState.X ? CellState.O : CellState.X;
}
