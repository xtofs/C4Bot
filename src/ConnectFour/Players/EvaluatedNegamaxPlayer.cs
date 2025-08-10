using System;
using System.Net;

namespace ConnectFour;

/// <summary>
/// A Negamax player enhanced with static position evaluation.
/// Uses Connect Four-specific heuristics to evaluate non-terminal positions.
/// </summary>
public class EvaluatedNegamaxPlayer : IPlayer
{
    /// <inheritdoc/>
    public string Name { get; }
    private readonly int maxDepth;

    /// <summary>
    /// Creates a new Enhanced Negamax player with the given name and search depth.
    /// </summary>
    /// <param name="name">The display name for the player.</param>
    /// <param name="maxDepth">The maximum search depth (default 6).</param>
    public EvaluatedNegamaxPlayer(string name, int maxDepth = 6)
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
            var score = -Negamax(newBoard, Opponent(player), maxDepth - 1, int.MinValue, int.MaxValue, player);
            if (score > bestScore)
            {
                bestScore = score;
                bestMove = move;
            }
        }
        return bestMove;
    }

    /// <summary>
    /// Negamax recursive search with alpha-beta pruning and static evaluation.
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
                // continue 
                break;
        }

        // Use static evaluation at leaf nodes
        if (depth == 0)
        {
            var eval = EvaluatePosition(board, originalPlayer);
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
                break;
            }
        }
        return max;
    }

    /// <summary>
    /// Evaluates a non-terminal position using Connect Four-specific heuristics.
    /// </summary>
    private static int EvaluatePosition(GameBoard board, CellState player)
    {
        var opponent = Opponent(player);
        int score = 0;

        // Center column preference - center control is crucial in Connect Four
        score += CountPiecesInColumn(board, 3, player) * 3;
        score -= CountPiecesInColumn(board, 3, opponent) * 3;

        // Evaluate threats (potential four-in-a-rows)
        score += CountThreats(board, player) * 50;
        score -= CountThreats(board, opponent) * 60; // Defending is slightly more important

        // Control of center region (columns 2-4)
        for (int col = 2; col <= 4; col++)
        {
            score += CountPiecesInColumn(board, col, player) * 2;
            score -= CountPiecesInColumn(board, col, opponent) * 2;
        }

        // Evaluate potential two-in-a-rows (building opportunities)
        score += CountTwoInARows(board, player) * 2;
        score -= CountTwoInARows(board, opponent) * 2;

        return score;
    }

    /// <summary>
    /// Counts the number of pieces a player has in a specific column.
    /// </summary>
    private static int CountPiecesInColumn(GameBoard board, int column, CellState player)
    {
        int count = 0;
        for (int row = 0; row < GameBoard.Rows; row++)
        {
            if (board[row, column] == player)
                count++;
        }
        return count;
    }

    /// <summary>
    /// Counts immediate threats (three-in-a-row with one empty space).
    /// </summary>
    private static int CountThreats(GameBoard board, CellState player)
    {
        int threats = 0;

        // Check all possible four-in-a-row combinations
        for (int row = 0; row < GameBoard.Rows; row++)
        {
            for (int col = 0; col < GameBoard.Columns; col++)
            {
                // Horizontal threats
                if (col <= GameBoard.Columns - 4)
                    threats += CountLineThreat(board, row, col, 0, 1, player);

                // Vertical threats  
                if (row <= GameBoard.Rows - 4)
                    threats += CountLineThreat(board, row, col, 1, 0, player);

                // Diagonal threats (/)
                if (row <= GameBoard.Rows - 4 && col <= GameBoard.Columns - 4)
                    threats += CountLineThreat(board, row, col, 1, 1, player);

                // Diagonal threats (\)
                if (row >= 3 && col <= GameBoard.Columns - 4)
                    threats += CountLineThreat(board, row, col, -1, 1, player);
            }
        }

        return threats;
    }

    /// <summary>
    /// Counts threats in a specific line (four consecutive positions).
    /// Returns 1 if exactly 3 pieces of player and 1 empty, 0 otherwise.
    /// </summary>
    private static int CountLineThreat(GameBoard board, int startRow, int startCol, int deltaRow, int deltaCol, CellState player)
    {
        int playerCount = 0;
        int emptyCount = 0;

        for (int i = 0; i < 4; i++)
        {
            int row = startRow + i * deltaRow;
            int col = startCol + i * deltaCol;
            
            var cell = board[row, col];
            if (cell == player)
                playerCount++;
            else if (cell == CellState.Empty)
                emptyCount++;
            else
                return 0; // Opponent piece blocks this line
        }

        return (playerCount == 3 && emptyCount == 1) ? 1 : 0;
    }

    /// <summary>
    /// Counts two-in-a-row patterns for building future threats.
    /// </summary>
    private static int CountTwoInARows(GameBoard board, CellState player)
    {
        int count = 0;

        for (int row = 0; row < GameBoard.Rows; row++)
        {
            for (int col = 0; col < GameBoard.Columns; col++)
            {
                // Horizontal two-in-a-rows
                if (col <= GameBoard.Columns - 4)
                    count += CountLineTwoInARow(board, row, col, 0, 1, player);

                // Vertical two-in-a-rows
                if (row <= GameBoard.Rows - 4)
                    count += CountLineTwoInARow(board, row, col, 1, 0, player);

                // Diagonal two-in-a-rows (/)
                if (row <= GameBoard.Rows - 4 && col <= GameBoard.Columns - 4)
                    count += CountLineTwoInARow(board, row, col, 1, 1, player);

                // Diagonal two-in-a-rows (\)
                if (row >= 3 && col <= GameBoard.Columns - 4)
                    count += CountLineTwoInARow(board, row, col, -1, 1, player);
            }
        }

        return count;
    }

    /// <summary>
    /// Counts two-in-a-row patterns in a specific line.
    /// Returns 1 if exactly 2 pieces of player and 2 empty, 0 otherwise.
    /// </summary>
    private static int CountLineTwoInARow(GameBoard board, int startRow, int startCol, int deltaRow, int deltaCol, CellState player)
    {
        int playerCount = 0;
        int emptyCount = 0;

        for (int i = 0; i < 4; i++)
        {
            int row = startRow + i * deltaRow;
            int col = startCol + i * deltaCol;
            
            var cell = board[row, col];
            if (cell == player)
                playerCount++;
            else if (cell == CellState.Empty)
                emptyCount++;
            else
                return 0; // Opponent piece blocks this line
        }

        return (playerCount == 2 && emptyCount == 2) ? 1 : 0;
    }

    private static CellState Opponent(CellState player) => player == CellState.X ? CellState.O : CellState.X;
}
