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
        var gamesPerMatch = int.TryParse(args[0], out var result) ? result : 100;

        var players = new IPlayer[]
        {
            new RandomPlayer("Random"),
            new NegamaxPlayer("Negamax-6", 6),                         // Basic evaluation 
            new NegamaxWithHeuristicPlayer("EnhancedNegamax-6", 6),    // Evaluation with heuristics
            new MonteCarloTreeSearchPlayer("MonteCarlo-1000", 1000),
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