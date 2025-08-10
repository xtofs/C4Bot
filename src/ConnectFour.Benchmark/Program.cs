using ConnectFour.Benchmark;
using ConnectFour;

Console.WriteLine("Connect Four AI Tournament");
Console.WriteLine("==========================");
Console.WriteLine();

// Configure tournament parameters
const int gamesPerMatch = 200; // Number of games each pair plays - increased for better statistics
const int mctsSimulations = 1000; // Simulations for MCTS

// Create AI players - simplified set focusing on key insights
var players = new List<IPlayer>
{
    new RandomPlayer("Random"),
    new NegamaxPlayer("Negamax-3", 3),   // Anomaly: surprisingly strong for shallow depth
    new NegamaxPlayer("Negamax-6", 6),   // Standard reference depth
    new EvaluatedNegamaxPlayer("EnhancedNegamax-4", 4), // Enhanced with static evaluation
    new EvaluatedNegamaxPlayer("EnhancedNegamax-6", 6), // Enhanced with static evaluation
    new NegamaxPlayer("Negamax-8", 8),   // Perfect vs random, demonstrates tactical completeness
    new MonteCarloTreeSearchPlayer("MonteCarlo-1000", mctsSimulations),  // Champion
};

Console.WriteLine($"Players: {string.Join(", ", players.Select(p => p.Name))}");
Console.WriteLine($"Games per match: {gamesPerMatch}");
Console.WriteLine();

// Run the tournament
var tournament = new Tournament();
var results = await tournament.RunTournamentAsync(players, gamesPerMatch);

// Print final summary
Console.WriteLine();
Tournament.PrintTournamentSummary(results);

// Analyze Negamax depth vs Random correlation
Console.WriteLine();
AnalyzeNegamaxDepthCorrelation(results);

Console.WriteLine();
Console.WriteLine("Tournament complete!");

static void AnalyzeNegamaxDepthCorrelation(List<MatchResult> results)
{
    Console.WriteLine("=== NEGAMAX DEPTH vs RANDOM ANALYSIS ===");
    Console.WriteLine();
    
    // Extract Negamax vs Random results
    var negamaxVsRandom = results
        .Where(r => (r.Player1Name.StartsWith("Negamax-") && r.Player2Name == "Random") ||
                   (r.Player2Name.StartsWith("Negamax-") && r.Player1Name == "Random"))
        .Select(r => 
        {
            if (r.Player1Name.StartsWith("Negamax-"))
            {
                var depth = int.Parse(r.Player1Name.Substring(8)); // Extract depth from "Negamax-X"
                var randomWins = r.Player2Wins;
                var totalGames = r.TotalGames;
                var lossRate = (double)randomWins / totalGames;
                return new { Depth = depth, LossRate = lossRate, RandomWins = randomWins, TotalGames = totalGames };
            }
            else
            {
                var depth = int.Parse(r.Player2Name.Substring(8));
                var randomWins = r.Player1Wins;
                var totalGames = r.TotalGames;
                var lossRate = (double)randomWins / totalGames;
                return new { Depth = depth, LossRate = lossRate, RandomWins = randomWins, TotalGames = totalGames };
            }
        })
        .OrderBy(x => x.Depth)
        .ToList();

    Console.WriteLine("Depth | Random Wins | Total Games | Loss Rate | Trend");
    Console.WriteLine("------|-------------|-------------|-----------|-------");
    
    foreach (var result in negamaxVsRandom)
    {
        Console.WriteLine($"{result.Depth,5} | {result.RandomWins,11} | {result.TotalGames,11} | {result.LossRate,8:P2} |");
    }
    
    // Calculate simple linear correlation
    if (negamaxVsRandom.Count >= 3)
    {
        var depths = negamaxVsRandom.Select(x => (double)x.Depth).ToArray();
        var lossRates = negamaxVsRandom.Select(x => x.LossRate).ToArray();
        
        var correlation = CalculateCorrelation(depths, lossRates);
        
        Console.WriteLine();
        Console.WriteLine($"Linear Correlation (Depth vs Loss Rate): {correlation:F4}");
        
        if (correlation < -0.7)
            Console.WriteLine("Strong negative correlation - Higher depth significantly reduces losses to random");
        else if (correlation < -0.3)
            Console.WriteLine("Moderate negative correlation - Higher depth reduces losses to random");
        else if (correlation > -0.1)
            Console.WriteLine("Weak/No correlation - Depth has minimal impact on losses to random");
        else
            Console.WriteLine("Some negative correlation - Higher depth somewhat reduces losses to random");
            
        // Look for the depth where losses become zero or minimal
        var zeroLosses = negamaxVsRandom.Where(x => x.RandomWins == 0).ToList();
        if (zeroLosses.Any())
        {
            var minDepthZeroLoss = zeroLosses.Min(x => x.Depth);
            Console.WriteLine($"First depth with zero losses to Random: {minDepthZeroLoss}");
        }
    }
}

static double CalculateCorrelation(double[] x, double[] y)
{
    if (x.Length != y.Length || x.Length == 0) return 0;
    
    var meanX = x.Average();
    var meanY = y.Average();
    
    var numerator = x.Zip(y, (xi, yi) => (xi - meanX) * (yi - meanY)).Sum();
    var denominatorX = Math.Sqrt(x.Sum(xi => Math.Pow(xi - meanX, 2)));
    var denominatorY = Math.Sqrt(y.Sum(yi => Math.Pow(yi - meanY, 2)));
    
    return denominatorX == 0 || denominatorY == 0 ? 0 : numerator / (denominatorX * denominatorY);
}
