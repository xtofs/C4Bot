using System;

namespace ConnectFour;

/// <summary>
/// The core Negamax algorithm with alpha-beta pruning, parameterized by search depth and position evaluator.
/// </summary>
public class NegamaxEngine
{
    private readonly IPositionEvaluator evaluator;

    public int MaxDepth { get; }

    /// <summary>
    /// Creates a new Negamax algorithm with the specified evaluator and search depth.
    /// </summary>
    /// <param name="evaluator">The position evaluator to use for leaf positions.</param>
    /// <param name="maxDepth">The maximum search depth.</param>
    public NegamaxEngine(IPositionEvaluator evaluator, int maxDepth)
    {
        this.evaluator = evaluator ?? throw new ArgumentNullException(nameof(evaluator));
        this.MaxDepth = maxDepth;
    }

    /// <summary>
    /// Finds the best move for the given player using Negamax search.
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
            var score = -Negamax(newBoard, Opponent(player), MaxDepth - 1, int.MinValue, int.MaxValue, player);
            if (score > bestScore)
            {
                bestScore = score;
                bestMove = move;
            }
        }
        
        return bestMove;
    }

    /// <summary>
    /// Negamax recursive search with alpha-beta pruning.
    /// </summary>
    private int Negamax(GameBoard board, CellState player, int depth, int alpha, int beta, CellState originalPlayer)
    {
        var result = board.GetGameResult();
        switch (result)
        {
            case GameResult.WinX:
                return player == CellState.X ? 10000 + depth : -10000 - depth;
            case GameResult.WinO:
                return player == CellState.O ? 10000 + depth : -10000 - depth;
            case GameResult.Draw:
                return 0;
            case GameResult.Ongoing:
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
            var score = -Negamax(newBoard, Opponent(player), depth - 1, -beta, -alpha, originalPlayer);
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
