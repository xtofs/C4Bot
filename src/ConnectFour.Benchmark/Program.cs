using ConnectFour.Benchmark;
using ConnectFour.Players;

internal class Program
{
    private static void Main(string[] args)
    {
        Console.WriteLine("Connect Four Tournament");
        Console.WriteLine("=======================");
        Console.WriteLine();

        // Configure tournament parameters
        var gamesPerMatch = (args.Length  > 0 && int.TryParse(args[0], out var result)) ? result : 100;

        var players = new IPlayer[]
        {
            new RandomPlayer("Random"),
            new MonteCarloTreeSearchPlayer("MonteCarlo-1000", 1000),    // MCTS with 1000 simulations
            new MinimaxPlayer("Minimax-6", 6),                          // Basic evaluation 
            new MinimaxWithHeuristicPlayer("EnhancedMinimax-6", 6, new BitboardPositionEvaluator()), // Bitboard-optimized heuristics
            new HybridPlayer("Hybrid-MCTS/Enhanced", 1000, 6, 14),      // MCTS opening + Enhanced Minimax endgame
        };

        Console.WriteLine($"Players: {string.Join(", ", players.Select(p => p.PlayerName))}");
        Console.WriteLine($"Games per match: {gamesPerMatch}");
        Console.WriteLine();

        // Run the tournament
        var results = Tournament.RunTournament(players, gamesPerMatch);

        // Print final summary
        Console.WriteLine();
        Tournament.PrintTournamentSummary(results);

        Console.WriteLine();
        Console.WriteLine("Tournament complete!");
    }
}