namespace ConnectFour.Benchmark;

using ConnectFour;
using ConnectFour.Players;

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
    public static List<MatchResult> RunTournament(IList<IPlayer> players, int gamesPerMatch)
    {
        var results = new List<MatchResult>();
        
        // Calculate total number of pairings: n * (n-1) / 2
        var totalPairings = players.Count * (players.Count - 1) / 2;
        
        Console.WriteLine($"Starting tournament with {players.Count} players, {gamesPerMatch} games per match");
        Console.WriteLine($"Total pairings: {totalPairings}");
        Console.WriteLine();

        var currentPairing = 0;
        for (var i = 0; i < players.Count; i++)
        {
            for (var j = i + 1; j < players.Count; j++)
            {
                currentPairing++;
                Console.Write($"Pairing {currentPairing}/{totalPairings}: {players[i].PlayerName} vs {players[j].PlayerName} - ");
                
                var result = RunMatch(players[i], players[j], gamesPerMatch);
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
    private static MatchResult RunMatch(IPlayer player1, IPlayer player2, int gameCount)
    {
        int player1Wins = 0, player2Wins = 0, draws = 0;
        var totalTime = TimeSpan.Zero;

        for (var game = 0; game < gameCount; game++)
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

                case GameResult.WinX:
                case GameResult.WinO:
                    // unreachable because `game % 2` is always either 0 or 1
                    throw new InvalidOperationException("Unexpected game result in tournament: " + result);
                    
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
        Span<int> buffer = stackalloc int[GameBoard.Columns];

        while (true)
        {
            var player = currentPlayer == CellState.X ? playerX : playerO;
            var move = player.ChooseMove(board, currentPlayer);

            // Validate the move is valid
            var availableMoves = board.GetAvailableMoves(buffer);
            if (! availableMoves.Contains(move))
            {
                throw new InvalidOperationException($"move {move} is not available {string.Join(", ", availableMoves.ToArray())}");
            }

            board = board.ApplyMove(move, currentPlayer);

            var state = board.GetGameState();
            if (state != GameState.Ongoing)
            {
                // Only return a GameResult for terminal states
                return state.ToGameResult();
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
        var playerStats = players.ToDictionary(p => p, p => new { Wins = 0, Losses = 0, Draws = 0, TotalGames = 0, TotalTime = TimeSpan.Zero });

        foreach (var result in results)
        {
            var p1Stats = playerStats[result.Player1Name];
            var p2Stats = playerStats[result.Player2Name];
            
            playerStats[result.Player1Name] = new 
            {
                Wins = p1Stats.Wins + result.Player1Wins,
                Losses = p1Stats.Losses + result.Player2Wins, 
                Draws = p1Stats.Draws + result.Draws,
                TotalGames = p1Stats.TotalGames + result.TotalGames,
                TotalTime = p1Stats.TotalTime + result.TotalTime
            };
            
            playerStats[result.Player2Name] = new
            {
                Wins = p2Stats.Wins + result.Player2Wins,
                Losses = p2Stats.Losses + result.Player1Wins,
                Draws = p2Stats.Draws + result.Draws, 
                TotalGames = p2Stats.TotalGames + result.TotalGames,
                TotalTime = p2Stats.TotalTime + result.TotalTime
            };
        }

        // Print overall standings
        var tableData = players
            .OrderByDescending(p => (double)playerStats[p].Wins / playerStats[p].TotalGames)
            .Select(player =>
            {
                var stats = playerStats[player];
                var winRate = (double)stats.Wins / stats.TotalGames;
                return new object[] { player, stats.Wins, stats.Losses, stats.Draws, winRate };
            })
            .ToList();

        PrintTable("Overall Standings:", 
            new[] { "Player", "Wins", "Losses", "Draws", "Win Rate" },
            new[] { 25, 4, 6, 5, 7 },
            new[] { "left", "right", "right", "right", "percent" },
            tableData);

        Console.WriteLine();

        // Print performance metrics
        var performanceData = players
            .OrderBy(p => playerStats[p].TotalGames > 0 ? playerStats[p].TotalTime.TotalMilliseconds / playerStats[p].TotalGames : double.MaxValue)
            .Select(player =>
            {
                var stats = playerStats[player];
                var avgTimePerGame = stats.TotalGames > 0 ? stats.TotalTime.TotalMilliseconds / stats.TotalGames : 0.0;
                var totalTimeSeconds = stats.TotalTime.TotalSeconds;
                return new object[] { player, stats.TotalGames, totalTimeSeconds, avgTimePerGame };
            })
            .ToList();

        PrintTable("Performance Metrics (Speed Rankings):", 
            new[] { "Player", "Games", "Total Time", "Avg/Game" },
            new[] { 25, 5, 10, 8 },
            new[] { "left", "right", "time", "time_ms" },
            performanceData);

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

        PrintMatrix("Head-to-Head Win Rate Matrix (Row vs Column):", players, players, matchLookup, 8);
    }

    /// <summary>
    /// Prints a formatted table with headers, data, and customizable column formatting.
    /// </summary>
    private static void PrintTable(string title, string[] headers, int[] columnWidths, string[] alignments, List<object[]> data)
    {
        Console.WriteLine(title);
        
        // Print header
        for (var i = 0; i < headers.Length; i++)
        {
            var separator = i > 0 ? " | " : "";
            var header = alignments[i] == "left" 
                ? headers[i].PadRight(columnWidths[i]) 
                : headers[i].PadLeft(columnWidths[i]);
            Console.Write($"{separator}{header}");
        }
        Console.WriteLine();

        // Print separator
        for (var i = 0; i < headers.Length; i++)
        {
            var separator = i > 0 ? " | " : "";
            Console.Write($"{separator}{new string('-', columnWidths[i])}");
        }
        Console.WriteLine();

        // Print data rows
        foreach (var row in data)
        {
            for (var i = 0; i < row.Length; i++)
            {
                var separator = i > 0 ? " | " : "";
                var value = row[i];
                string formattedValue = alignments[i] switch
                {
                    "percent" when value is double d => d.ToString("P1"),
                    "time" when value is double d => $"{d:F1}s",
                    "time_ms" when value is double d => $"{d:F0}ms",
                    _ => value?.ToString() ?? ""
                };
                var alignedValue = alignments[i] == "left" 
                    ? formattedValue.PadRight(columnWidths[i]) 
                    : formattedValue.PadLeft(columnWidths[i]);
                Console.Write($"{separator}{alignedValue}");
            }
            Console.WriteLine();
        }
    }

    /// <summary>
    /// Prints a formatted matrix with row and column headers.
    /// </summary>
    private static void PrintMatrix(string title, List<string> rowHeaders, List<string> columnHeaders, Dictionary<(string, string), MatchResult> matchData, int cellWidth)
    {
        Console.WriteLine(title);
        Console.WriteLine();

        // Print header
        Console.Write("Player".PadRight(15));
        foreach (var colHeader in columnHeaders)
        {
            var truncatedHeader = colHeader[..Math.Min(cellWidth, colHeader.Length)];
            Console.Write($" | {truncatedHeader.PadLeft(cellWidth)}");
        }
        Console.WriteLine();

        // Print separator
        Console.Write("".PadRight(15, '-'));
        foreach (var _ in columnHeaders)
        {
            Console.Write($" | {new string('-', cellWidth)}");
        }
        Console.WriteLine();

        // Print matrix rows
        foreach (var rowHeader in rowHeaders)
        {
            Console.Write($"{rowHeader[..Math.Min(15, rowHeader.Length)],-15}");
            
            foreach (var colHeader in columnHeaders)
            {
                string cellValue;
                if (rowHeader == colHeader)
                {
                    cellValue = "    -   ";
                }
                else if (matchData.TryGetValue((rowHeader, colHeader), out var match))
                {
                    cellValue = $"{match.Player1WinRate:P1}";
                }
                else
                {
                    cellValue = "  N/A   ";
                }
                
                Console.Write($" | {cellValue.PadLeft(cellWidth)}");
            }
            Console.WriteLine();
        }
    }
}
