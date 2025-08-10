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
        (int row, int col)? lastMove = null;
        
        while (true)
        {
            PrintBoard(board, lastMove);
            var turn = board.HalfTurn % 2 == 0 ? 0 : 1;
            var player = turn == 0 ? playerX : playerO;
            var state = turn == 0 ? CellState.X : CellState.O;
            
            // Only show "to move" message for non-interactive players
            // Interactive players will get their own combined prompt
            if (!(player is InteractivePlayer))
            {
                Console.WriteLine($"{player.PlayerName} to move ({(state == CellState.X ? "X" : "O")})");
            }
            
            int move = player.ChooseMove(board, state);
            
            // Calculate where the piece will land
            int landingRow = -1;
            for (int row = 0; row < GameBoard.Rows; row++)
            {
                if (board[row, move] == CellState.Empty)
                {
                    landingRow = row;
                    break;
                }
            }
            lastMove = landingRow >= 0 ? (landingRow, move) : null;
            
            board = board.ApplyMove(move, state);
            if (board.HasGameEnded(out var result, out var winningCells))
            {
                if (result == GameResult.WinX || result == GameResult.WinO)
                {
                    PrintBoard(board, lastMove, winningCells);
                    Console.WriteLine($"{player.PlayerName} wins!");
                }
                else if (result == GameResult.Draw)
                {
                    PrintBoard(board, lastMove);
                    Console.WriteLine("Draw!");
                }
                break;
            }
        }
    }

    /// <summary>
    /// Prints the game board to the console, optionally highlighting the last move and winning cells.
    /// </summary>
    /// <param name="board">The game board to print.</param>
    /// <param name="lastMove">The last move to highlight (row, col).</param>
    /// <param name="winningCells">Optional set of cells to highlight (e.g., winning four).</param>
    private static void PrintBoard(GameBoard board, (int row, int col)? lastMove = null, HashSet<(int row, int col)>? winningCells = null)
    {
        for (int row = GameBoard.Rows - 1; row >= 0; row--)
        {
            for (int col = 0; col < GameBoard.Columns; col++)
            {
                var cell = board[row, col];
                char c = cell == CellState.Empty ? '.' : (cell == CellState.X ? 'X' : 'O');
                
                if (winningCells != null && winningCells.Contains((row, col)))
                {
                    // Highlight winning cells in green
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write($"{c} ");
                    Console.ResetColor();
                }
                else if (lastMove.HasValue && lastMove.Value.row == row && lastMove.Value.col == col)
                {
                    // Highlight last move in orange
                    Console.ForegroundColor = ConsoleColor.DarkYellow;
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
        
        // Add column numbers for human players
        Console.WriteLine("1 2 3 4 5 6 7");
        Console.WriteLine();
    }
    // ...existing code...
}
