namespace ConnectFour;

/// <summary>
/// Example demonstrating improved type safety with the new enum design.
/// This shows how the new enums prevent common mistakes and improve code clarity.
/// </summary>
public static class TypeSafetyExample
{
    /// <summary>
    /// OLD WAY: Using CellState for players - error prone!
    /// </summary>
    public static void OldWayProblematic()
    {
        var board = new GameBoard();
        
        // ❌ This compiles but is semantically wrong!
        CellState currentPlayer = CellState.Empty; // Should never happen for a player!
        
        // ❌ Easy to make mistakes - need to remember to check for Empty
        if (currentPlayer != CellState.Empty)
        {
            board = board.ApplyMove(3, currentPlayer);
        }
        
        // ❌ GameState.Ongoing mixed with terminal states
        if (board.HasGameEnded(out var result, out var cells))
        {
            if (result == GameState.WinX || result == GameState.WinO)
            {
                // Handle win - but result could theoretically be Ongoing here (confusing!)
            }
        }
    }

    /// <summary>
    /// NEW WAY: Using Player enum - type safe!
    /// </summary>
    public static void NewWayTypeSafe()
    {
        var board = new GameBoard();
        
        // ✅ Cannot accidentally set player to Empty
        Player currentPlayer = Player.X;
        
        // ✅ No need to check for Empty - impossible by design!
        board = board.ApplyMove(3, currentPlayer);
        
        // ✅ Safe conversion when needed for display
        CellState cellToPlace = currentPlayer.ToCellState();
        
        // ✅ Get opponent safely
        Player nextPlayer = currentPlayer.Opponent();
        
        // ✅ Clear intent: only terminal results, null if ongoing
        GameResult? gameResult = board.GetGameResult(out var winningCells);
        
        if (gameResult.HasValue)
        {
            Player? winner = gameResult.Value.GetWinningPlayer();
            if (winner.HasValue)
            {
                Console.WriteLine($"Player {winner} wins!");
            }
            else
            {
                Console.WriteLine("It's a draw!");
            }
        }
        else
        {
            Console.WriteLine("Game is still ongoing");
        }
        
        // ✅ Type-safe threat detection
        List<(int row, int col)> threats = board.FindThreats(currentPlayer.Opponent());
        if (threats.Count > 0)
        {
            Console.WriteLine($"Warning: {threats.Count} threats detected!");
        }
    }

    /// <summary>
    /// Demonstrates migration path: both APIs can coexist.
    /// </summary>
    public static void MigrationExample()
    {
        var board = new GameBoard();
        Player player = Player.X;
        
        // Can use new API
        board = board.ApplyMove(3, player);
        
        // Old API still works for legacy code
        board = board.ApplyMove(4, CellState.O);
        
        // New API is more expressive
        var threats = board.FindThreats(player);
        
        // Can convert between representations when needed
        CellState legacyPlayer = player.ToCellState();
        Player modernPlayer = legacyPlayer.ToPlayer();
    }

    /// <summary>
    /// Shows how method signatures become clearer.
    /// </summary>
    public static void ClearerMethodSignatures()
    {
        // OLD: Unclear what values are valid
        // ProcessMove(CellState.Empty); // ❌ Compiles but wrong!
        
        // NEW: Type system enforces correctness
        ProcessMove(Player.X); // ✅ Can only pass valid players
        
        // OLD: Mixed concerns in return type
        // if (result == GameState.Ongoing) { /* not actually a result! */ }
        
        // NEW: Clear separation
        var result = GetGameOutcome();
        if (result.HasValue)
        {
            // Only terminal states reach here
            HandleGameEnd(result.Value);
        }
        else
        {
            // Game continues
            ContinueGame();
        }
    }

    private static void ProcessMove(Player player)
    {
        // Implementation knows player is never Empty
        Console.WriteLine($"Processing move for {player}");
    }

    private static GameResult? GetGameOutcome()
    {
        // Simulated - return null for ongoing, GameResult for ended
        return null;
    }

    private static void HandleGameEnd(GameResult result)
    {
        Console.WriteLine($"Game ended: {result}");
    }

    private static void ContinueGame()
    {
        Console.WriteLine("Game continues...");
    }
}
