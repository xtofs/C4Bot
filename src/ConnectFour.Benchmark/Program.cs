using ConnectFour.Benchmark;
using ConnectFour.Players;

Console.WriteLine("Connect Four AI Tournament");
Console.WriteLine("==========================");
Console.WriteLine();

// Configure tournament parameters
const int gamesPerMatch = 50; // Reduced for quicker testing
const int mctsSimulations = 1000; // Simulations for MCTS

// Create AI players - clean algorithm implementations
var players = new List<IPlayer>
{
    new RandomPlayer("Random"),
    new NegamaxPlayer("Negamax-6", 6),                    // Basic evaluation (terminal only)
    new NegamaxWithHeuristicPlayer("EnhancedNegamax-6", 6),    // Smart evaluation with heuristics
    new MonteCarloTreeSearchPlayer("MonteCarlo-1000", mctsSimulations),
};

Console.WriteLine($"Players: {string.Join(", ", players.Select(p => p.PlayerName))}");
Console.WriteLine($"Games per match: {gamesPerMatch}");
Console.WriteLine();

// Run the tournament
var tournament = new Tournament();
var results = await Tournament.RunTournamentAsync(players, gamesPerMatch);

// Print final summary
Console.WriteLine();
Tournament.PrintTournamentSummary(results);

Console.WriteLine();
Console.WriteLine("Tournament complete!");
