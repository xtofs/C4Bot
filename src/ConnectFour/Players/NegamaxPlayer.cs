using System;

namespace ConnectFour;

/// <summary>
/// A player that uses the Negamax algorithm to select moves.
/// </summary>
public class NegamaxPlayer : IPlayer
{
    /// <inheritdoc/>
    public string Name { get; }
    private readonly int maxDepth;

    /// <summary>
    /// Creates a new Negamax player with the given name and search depth.
    /// </summary>
    /// <param name="name">The display name for the player.</param>
    /// <param name="maxDepth">The maximum search depth (default 6).</param>
    public NegamaxPlayer(string name, int maxDepth = 6)
    {
        Name = name;
        this.maxDepth = maxDepth;
    }

    /// <inheritdoc/>
    public int ChooseMove(GameBoard board, CellState player)
    {
        var bestMove = -1;
        var bestScore = int.MinValue;
        foreach (var move in board.GetAvailableMoves())
        {
            var newBoard = board.ApplyMove(move, player);
            var score = -Negamax(newBoard, Opponent(player), maxDepth - 1, int.MinValue, int.MaxValue);
            if (score > bestScore)
            {
                bestScore = score;
                bestMove = move;
            }
        }
        return bestMove;
    }

    /// <summary>
    /// Negamax recursive search for best move.
    /// </summary>
    private int Negamax(GameBoard board, CellState player, int depth, int alpha, int beta)
    {
        var result = board.GetGameResult();
        if (result == GameResult.WinX)
        {
            return player == CellState.X ? 10000 + depth : -10000 - depth;
        }
        if (result == GameResult.WinO)
        {
            return player == CellState.O ? 10000 + depth : -10000 - depth;
        }
        if (result == GameResult.Draw || depth == 0)
        {
            return 0;
        }

        var max = int.MinValue;
        foreach (var move in board.GetAvailableMoves())
        {
            var newBoard = board.ApplyMove(move, player);
            var score = -Negamax(newBoard, Opponent(player), depth - 1, -beta, -alpha);
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
                break;
            }
        }
        return max;
    }

    private static CellState Opponent(CellState player) => player == CellState.X ? CellState.O : CellState.X;
}
