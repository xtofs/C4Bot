using ConnectFour;
using ConnectFour.Players;

namespace ConnectFour.ConsoleApp;

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Welcome to Connect Four!");

        // a list of algorithm choices
        (string AlgorithmName, Func<string, IPlayer> Factory)[] playerChoices = [
          ("Console", name => new InteractivePlayer(name)),
          ("Random", name => new RandomPlayer(name)),
          ("MonteCarloTreeSearch (1000)", name => new MonteCarloTreeSearchPlayer(name, 1000)),
          ("Minimax (6)", name => new MinimaxPlayer(name, 6)),
          ("EnhancedMinimax (6)", name => new MinimaxWithHeuristicPlayer(name, 6)),
        ];

        IPlayer playerX, playerO;
        if (args.Length >= 2 &&
            int.TryParse(args[0], out var a0) && a0 >= 1 && a0 <= playerChoices.Length &&
            int.TryParse(args[1], out var a1) && a1 >= 1 && a1 <= playerChoices.Length)
        {
            playerX = CreatePlayerFromArg(playerChoices, a0, "Player X");
            playerO = CreatePlayerFromArg(playerChoices, a1, "Player O");

            Console.WriteLine($"Player X {playerX.PlayerName} {playerX.AlgorithmName}");
            Console.WriteLine($"Player O {playerO.PlayerName} {playerO.AlgorithmName}");
        }
        else if (args.Length != 2)
        {
            Console.WriteLine("Please provide two player selections as command line arguments. Or none for interactive selection.");
            Console.WriteLine("Available algorithms:");
            foreach (var ((name, _), i) in playerChoices.Select((a, i) => (a, i + 1)))
            {
                Console.WriteLine($" - {i}, {name}");
            }
            throw new ArgumentException();
        }
        else
        {
            playerX = SelectPlayer(playerChoices, "Player X");
            playerO = SelectPlayer(playerChoices, "Player O");
        }

        GameRunner.RunGame(playerX, playerO);
    }


    static IPlayer CreatePlayerFromArg((string AlgorithmName, Func<string, IPlayer> Factory)[] choices, int arg, string name)
    {
        var choice = choices[arg - 1];

        return choice.Factory(name);
    }

    static IPlayer SelectPlayer((string AlgorithmName, Func<string, IPlayer> Factory)[] choices, string playerName)
    {
        Console.WriteLine($"Select algorithm for {playerName}:");
        Console.WriteLine($"Enter Number between 1 and {choices.Length}:");
        foreach (var ((name, _), i) in choices.Select((a, i) => (a, i + 1)))
        {
            Console.WriteLine($" - {i}, {name}");
        }

        while (true)
        {
            var input = Console.ReadLine();
            if (int.TryParse(input, out var choiceIndex) && choiceIndex >= 1 && choiceIndex <= choices.Length)
            {
                return choices[choiceIndex - 1].Factory(playerName);
            }

            Console.WriteLine($"Invalid selection. Enter Number between 1 and {choices.Length}:");
        }
    }
}
