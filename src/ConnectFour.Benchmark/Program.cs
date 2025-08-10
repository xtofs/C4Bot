using ConnectFour.Benchmark;
using ConnectFour;

Console.WriteLine("Connect Four AI Tournament");
Console.WriteLine("==========================");
Console.WriteLine();

// Configure tournament parameters
const int gamesPerMatch = 100; // Number of games each pair plays
const int negamaxDepth = 6;   // Depth for Negamax algorithm  
const int mctsSimulations = 1000; // Simulations for MCTS

// Create AI players
var players = new List<IPlayer>
{
    new RandomPlayer("Random"),
    new NegamaxPlayer("Negamax-6", negamaxDepth),
    new MonteCarloTreeSearchPlayer("MCTS-1000", mctsSimulations),
    // Add more configurations if desired
    new NegamaxPlayer("Negamax-4", 4),
    new MonteCarloTreeSearchPlayer("MCTS-500", 500)
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

Console.WriteLine();
Console.WriteLine("Tournament complete!");
