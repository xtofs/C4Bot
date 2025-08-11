namespace ConnectFour;

using ConnectFour.Players;

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
        var moveHistory = new List<int>(); // Track moves for notation

        while (true)
        {
            PrintBoard(board, lastMove);
            var turn = board.HalfMoveCount % 2 == 0 ? 0 : 1;
            var player = turn == 0 ? playerX : playerO;
            var state = turn == 0 ? CellState.X : CellState.O;

            var move = player.ChooseMove(board, state);

            if (player is not InteractivePlayer)
            {
                Console.WriteLine($"{(state == CellState.X ? "X" : "O")} played {move + 1}");
            }

            moveHistory.Add(move); // Track the move for notation
            board = board.ApplyMove(move, state, out var lastMoveCoords);
            lastMove = lastMoveCoords;
            if (board.HasGameEnded(out var result, out var winningCells))
            {
                if (result == GameResult.WinX || result == GameResult.WinO)
                {
                    PrintBoard(board, lastMove, winningCells);
                    Console.WriteLine($"{(result == GameResult.WinX ? playerX.PlayerName : playerO.PlayerName)} wins!");
                }
                else if (result == GameResult.Draw)
                {
                    PrintBoard(board, lastMove);
                    Console.WriteLine("Draw!");
                }
                
                // Print game moves for replay/analysis
                var moves = GameBoard.ToMoves(moveHistory);
                Console.WriteLine($"Game moves: {moves}");
                Console.WriteLine($"To replay this game, use: dotnet run --moves \"{moves}\"");
                break;
            }
        }
    }

    /// <summary>
    /// Runs a Connect Four game between two players from a pre-loaded board position.
    /// </summary>
    /// <param name="playerX">The player for X.</param>
    /// <param name="playerO">The player for O.</param>
    /// <param name="startingBoard">The pre-loaded board position to continue from.</param>
    public static void RunGame(IPlayer playerX, IPlayer playerO, GameBoard startingBoard)
    {
        var board = startingBoard;
        (int row, int col)? lastMove = null;
        var moveHistory = new List<int>(); // Track moves for notation

        while (true)
        {
            PrintBoard(board, lastMove);
            var turn = board.HalfMoveCount % 2 == 0 ? 0 : 1;
            var player = turn == 0 ? playerX : playerO;
            var state = turn == 0 ? CellState.X : CellState.O;

            var move = player.ChooseMove(board, state);

            if (player is not InteractivePlayer)
            {
                Console.WriteLine($"{(state == CellState.X ? "X" : "O")} played {move + 1}");
            }

            moveHistory.Add(move); // Track the move for notation
            board = board.ApplyMove(move, state, out var lastMoveCoords);
            lastMove = lastMoveCoords;
            if (board.HasGameEnded(out var result, out var winningCells))
            {
                if (result == GameResult.WinX || result == GameResult.WinO)
                {
                    PrintBoard(board, lastMove, winningCells);
                    Console.WriteLine($"{(result == GameResult.WinX ? playerX.PlayerName : playerO.PlayerName)} wins!");
                }
                else if (result == GameResult.Draw)
                {
                    PrintBoard(board, lastMove);
                    Console.WriteLine("Draw!");
                }
                
                // Print game moves for replay/analysis
                var moves = GameBoard.ToMoves(moveHistory);
                Console.WriteLine($"Game moves: {moves}");
                Console.WriteLine($"To replay this game, use: dotnet run --moves \"{moves}\"");
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
        var grid = board.ToArray();
        
        for (int row = GameBoard.Rows - 1; row >= 0; row--)
        {
            for (int col = 0; col < GameBoard.Columns; col++)
            {
                char c = grid[row, col] switch
                {
                    CellState.X => 'X',
                    CellState.O => 'O',
                    _ => '.'
                };

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
