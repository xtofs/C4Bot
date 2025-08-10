namespace ConnectFour.Players;

/// <summary>
/// Interface for position evaluation strategies.
/// </summary>
public interface IPositionEvaluator
{
    /// <summary>
    /// Evaluates a game position and returns a score from the specified player's perspective.
    /// </summary>
    /// <param name="board">The game board to evaluate.</param>
    /// <param name="player">The player from whose perspective to evaluate.</param>
    /// <returns>The evaluation score (positive favors the player, negative favors opponent).</returns>
    int Evaluate(GameBoard board, CellState player);
}

/// <summary>
/// Basic evaluator that provides no static evaluation of positions.
/// Always returns 0, relying solely on the search tree to find terminal positions.
/// </summary>
public class BasicPositionEvaluator : IPositionEvaluator
{
    public int Evaluate(GameBoard board, CellState player)
    {
        return 0; // No static evaluation - purely tree-search based
    }
}

/// <summary>
/// Enhanced evaluator with Connect Four-specific heuristics.
/// The heuristics provide:
///   Center column preference (columns 3-4 most valuable)
///   Threat detection (3-in-a-row patterns worth 50 points)
///   Defensive prioritization (blocking threats worth 60 points)
///   Two-in-a-row building (foundation patterns worth 2 points)
/// </summary>
public class ConnectFourPositionEvaluator : IPositionEvaluator
{
    public int Evaluate(GameBoard board, CellState player)
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

    private static int CountThreats(GameBoard board, CellState player)
    {
        int threats = 0;

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
