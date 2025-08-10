using System.Diagnostics;
using ConnectFour;

namespace ConnectFour.Benchmark;

/// <summary>
/// Manages tournaments between AI players and collects statistics.
/// </summary>
public class Tournament
{
    /// <summary>
    /// Runs a tournament between all pairs of the given players.
    /// </summary>
    /// <param name="players">The players to compete.</param>
    /// <param name="gamesPerMatch">Number of games each pair should play.</param>
    /// <returns>Results for each matchup.</returns>
    public static async Task<List<MatchResult>> RunTournamentAsync(IList<IPlayer> players, int gamesPerMatch)
    {
        var results = new List<MatchResult>();
        
        Console.WriteLine($"Starting tournament with {players.Count} players, {gamesPerMatch} games per match");
        Console.WriteLine();

        for (int i = 0; i < players.Count; i++)
        {
            for (int j = i + 1; j < players.Count; j++)
            {
                var result = await RunMatchAsync(players[i], players[j], gamesPerMatch);
                results.Add(result);
                
                PrintMatchResult(result);
                Console.WriteLine();
            }
        }

        return results;
    }

    /// <summary>
    /// Runs a match between two players for the specified number of games.
    /// </summary>
    private static async Task<MatchResult> RunMatchAsync(IPlayer player1, IPlayer player2, int gameCount)
    {
        await Task.CompletedTask;

        int player1Wins = 0, player2Wins = 0, draws = 0;
        var totalTime = TimeSpan.Zero;

        Console.Write($"Match: {player1.PlayerName} vs {player2.PlayerName} - ");

        for (int game = 0; game < gameCount; game++)
        {
            // Alternate who goes first
            var (p1, p2) = game % 2 == 0 ? (player1, player2) : (player2, player1);
            
            var gameStart = DateTime.UtcNow;
            var result = PlayGame(p1, p2);
            var gameTime = DateTime.UtcNow - gameStart;
            totalTime += gameTime;

            // Count wins based on original player order
            switch (result)
            {
                case GameResult.WinX when game % 2 == 0: // player1 was X
                case GameResult.WinO when game % 2 == 1: // player1 was O
                    player1Wins++;
                    break;

                case GameResult.WinX when game % 2 == 1: // player2 was X
                case GameResult.WinO when game % 2 == 0: // player2 was O
                    player2Wins++;
                    break;

                case GameResult.Draw:
                    draws++;
                    break;

                // unreachable. added for exhaustive pattern matching
                case GameResult.Ongoing:
                case GameResult.WinO:
                case GameResult.WinX:
                default:                    
                    break;
            }

            // Progress indicator
            if ((game + 1) % Math.Max(1, gameCount / 10) == 0)
            {
                Console.Write(".");
            }
        }

        Console.WriteLine($" Done!");

        return new MatchResult(
            player1.PlayerName,
            player2.PlayerName, 
            player1Wins,
            player2Wins,
            draws,
            gameCount,
            totalTime);
    }

    /// <summary>
    /// Plays a single game between two players.
    /// </summary>
    private static GameResult PlayGame(IPlayer playerX, IPlayer playerO)
    {
        var board = new GameBoard();
        var currentPlayer = CellState.X;
        Span<int> availableMoves = stackalloc int[GameBoard.Columns];
        
        while (true)
        {
            var player = currentPlayer == CellState.X ? playerX : playerO;
            var move = player.ChooseMove(board, currentPlayer);
            
            // Validate the move is valid
            board.GetAvailableMoves(availableMoves, out int availableCount);
            bool isValidMove = false;
            for (int i = 0; i < availableCount; i++)
            {
                if (availableMoves[i] == move)
                {
                    isValidMove = true;
                    break;
                }
            }
            
            if (!isValidMove)
            {
                // If the player made an invalid move, they lose
                return currentPlayer == CellState.X ? GameResult.WinO : GameResult.WinX;
            }
            
            board = board.ApplyMove(move, currentPlayer);
            
            var result = board.GetGameResult();
            if (result != GameResult.Ongoing)
            {
                return result;
            }
            
            currentPlayer = currentPlayer == CellState.X ? CellState.O : CellState.X;
        }
    }

    /// <summary>
    /// Prints the results of a match in a formatted way.
    /// </summary>
    private static void PrintMatchResult(MatchResult result)
    {
        Console.WriteLine($"Results: {result.Player1Name} vs {result.Player2Name}");
        Console.WriteLine($"  {result.Player1Name}: {result.Player1Wins} wins ({result.Player1WinRate:P1})");
        Console.WriteLine($"  {result.Player2Name}: {result.Player2Wins} wins ({result.Player2WinRate:P1})");
        Console.WriteLine($"  Draws: {result.Draws} ({result.DrawRate:P1})");
        Console.WriteLine($"  Total time: {result.TotalTime.TotalSeconds:F1}s, Avg per game: {result.AverageGameTime.TotalMilliseconds:F0}ms");
    }

    /// <summary>
    /// Prints a summary table of all tournament results.
    /// </summary>
    public static void PrintTournamentSummary(List<MatchResult> results)
    {
        Console.WriteLine("=== TOURNAMENT SUMMARY ===");
        Console.WriteLine();
        
        // Get all unique players
        var players = results
            .SelectMany(r => new[] { r.Player1Name, r.Player2Name })
            .Distinct()
            .OrderBy(p => p)
            .ToList();

        // Calculate overall statistics for each player
        var playerStats = players.ToDictionary(p => p, p => new { Wins = 0, Losses = 0, Draws = 0, TotalGames = 0 });

        foreach (var result in results)
        {
            var p1Stats = playerStats[result.Player1Name];
            var p2Stats = playerStats[result.Player2Name];
            
            playerStats[result.Player1Name] = new 
            {
                Wins = p1Stats.Wins + result.Player1Wins,
                Losses = p1Stats.Losses + result.Player2Wins, 
                Draws = p1Stats.Draws + result.Draws,
                TotalGames = p1Stats.TotalGames + result.TotalGames
            };
            
            playerStats[result.Player2Name] = new
            {
                Wins = p2Stats.Wins + result.Player2Wins,
                Losses = p2Stats.Losses + result.Player1Wins,
                Draws = p2Stats.Draws + result.Draws, 
                TotalGames = p2Stats.TotalGames + result.TotalGames
            };
        }

        // Print overall standings
        Console.WriteLine("Overall Standings:");
        Console.WriteLine("Player                    | Wins | Losses | Draws | Win Rate");
        Console.WriteLine("--------------------------|------|--------|-------|----------");
        
        foreach (var player in players.OrderByDescending(p => (double)playerStats[p].Wins / playerStats[p].TotalGames))
        {
            var stats = playerStats[player];
            var winRate = (double)stats.Wins / stats.TotalGames;
            Console.WriteLine($"{player,-25} | {stats.Wins,4} | {stats.Losses,6} | {stats.Draws,5} | {winRate,7:P1}");
        }

        Console.WriteLine();
        PrintHeadToHeadMatrix(results, players);
    }

    /// <summary>
    /// Prints a head-to-head matrix showing win rates between all player pairs.
    /// </summary>
    private static void PrintHeadToHeadMatrix(List<MatchResult> results, List<string> players)
    {
        Console.WriteLine("Head-to-Head Win Rate Matrix (Row vs Column):");
        Console.WriteLine();

        // Create a lookup for match results
        var matchLookup = new Dictionary<(string, string), MatchResult>();
        foreach (var result in results)
        {
            matchLookup[(result.Player1Name, result.Player2Name)] = result;
            // Create reverse lookup with swapped win rates
            matchLookup[(result.Player2Name, result.Player1Name)] = new MatchResult(
                result.Player2Name,
                result.Player1Name,
                result.Player2Wins,
                result.Player1Wins,
                result.Draws,
                result.TotalGames,
                result.TotalTime
            );
        }

        // Print header
        Console.Write("Player".PadRight(15));
        foreach (var colPlayer in players)
        {
            Console.Write($" | {colPlayer[..Math.Min(8, colPlayer.Length)],8}");
        }
        Console.WriteLine();

        // Print separator
        Console.Write("".PadRight(15, '-'));
        foreach (var _ in players)
        {
            Console.Write(" | --------");
        }
        Console.WriteLine();

        // Print matrix rows
        foreach (var rowPlayer in players)
        {
            Console.Write($"{rowPlayer[..Math.Min(15, rowPlayer.Length)],-15}");
            
            foreach (var colPlayer in players)
            {
                if (rowPlayer == colPlayer)
                {                    
                    Console.Write(" |    -    ");
                }
                else if (matchLookup.TryGetValue((rowPlayer, colPlayer), out var match))
                {
                    var winRate = match.Player1WinRate;
                    Console.Write($" | {winRate,7:P1}");
                }
                else
                {
                    Console.Write(" |   N/A   ");
                }
            }
            Console.WriteLine();
        }
    }
}
