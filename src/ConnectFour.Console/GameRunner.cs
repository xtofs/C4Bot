namespace ConnectFour;

/// <summary>
/// Provides the main game loop and board printing for Connect Four.
/// </summary>
public static class GameRunner
{
    /// <summary>
    /// Runs a Connect Four game between two players, handling turns and output.
    /// </summary>
    /// <param name="playerX">The player for X.</param>
    /// <param name="playerO">The player for O.</param>
    public static void RunGame(IPlayer playerX, IPlayer playerO)
    {
        var board = new GameBoard();
        while (true)
        {
            PrintBoard(board);
            var turn = board.HalfTurn % 2 == 0 ? 0 : 1;
            var player = turn == 0 ? playerX : playerO;
            var state = turn == 0 ? CellState.X : CellState.O;
            Console.WriteLine($"{player.PlayerName} ({player.GetType().Name}) to move ({(state == CellState.X ? "X" : "O")})");
            int move = player.ChooseMove(board, state);
            board = board.ApplyMove(move, state);
            if (board.HasGameEnded(out var result, out var winningCells))
            {
                if (result == GameResult.WinX || result == GameResult.WinO)
                {
                    PrintBoard(board, winningCells);
                    Console.WriteLine($"{player.PlayerName} wins!");
                }
                else if (result == GameResult.Draw)
                {
                    PrintBoard(board);
                    Console.WriteLine("Draw!");
                }
                break;
            }
        }
    }

    /// <summary>
    /// Prints the game board to the console, optionally highlighting winning cells.
    /// </summary>
    /// <param name="board">The game board to print.</param>
    /// <param name="highlight">Optional set of cells to highlight (e.g., winning four).</param>
    private static void PrintBoard(GameBoard board, HashSet<(int row, int col)>? highlight = null)
    {
        for (int row = GameBoard.Rows - 1; row >= 0; row--)
        {
            for (int col = 0; col < GameBoard.Columns; col++)
            {
                var cell = board[row, col];
                char c = cell == CellState.Empty ? '.' : (cell == CellState.X ? 'X' : 'O');
                if (highlight != null && highlight.Contains((row, col)))
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.Write($"{c} ");
                    Console.ResetColor();
                }
                else
                {
                    Console.Write($"{c} ");
                }
            }
            Console.WriteLine();
        }
        Console.WriteLine();
    }
    // ...existing code...
}
