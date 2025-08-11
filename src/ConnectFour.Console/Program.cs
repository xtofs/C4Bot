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
          ("Interactive", name => new InteractivePlayer(name)),
          ("Random", name => new RandomPlayer(name)),
          ("MonteCarloTreeSearch (1000)", name => new MonteCarloTreeSearchPlayer(name, 1000)),
          ("Minimax (6)", name => new MinimaxPlayer(name, 6)),
          ("EnhancedMinimax (6)", name => new MinimaxWithHeuristicPlayer(name, 6, new BitboardPositionEvaluator())),
        ];

        IPlayer playerX, playerO;
        
        // Check for --moves argument
        if (args.Length >= 2 && args[0] == "--moves")
        {
            try 
            {
                var board = GameBoard.FromMoves(args[1]);
                Console.WriteLine($"Loaded position from moves: {args[1]}");
                Console.WriteLine($"Move count: {board.HalfMoveCount}");
                Console.WriteLine();
                
                // Print the current board state
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
                        Console.Write($"{c} ");
                    }
                    Console.WriteLine();
                }
                Console.WriteLine("1 2 3 4 5 6 7");
                Console.WriteLine();
                
                // Check if the game has already ended
                if (board.HasGameEnded(out var result, out var winningCells))
                {
                    if (result == GameResult.WinX)
                        Console.WriteLine("Player X has already won!");
                    else if (result == GameResult.WinO)  
                        Console.WriteLine("Player O has already won!");
                    else if (result == GameResult.Draw)
                        Console.WriteLine("Game is already a draw!");
                    return;
                }
                
                Console.WriteLine("This position can be continued interactively...");
                Console.WriteLine();
                
                // Allow player selection to continue the game from this position
                playerX = SelectPlayer(playerChoices, "Player X");
                playerO = SelectPlayer(playerChoices, "Player O");
                
                GameRunner.RunGame(playerX, playerO, board);
                return;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading moves: {ex.Message}");
                return;
            }
        }
        else if (args.Length >= 2 &&
            int.TryParse(args[0], out var a0) && a0 >= 1 && a0 <= playerChoices.Length &&
            int.TryParse(args[1], out var a1) && a1 >= 1 && a1 <= playerChoices.Length)
        {
            playerX = CreatePlayerFromArg(playerChoices, a0, "Player X");
            playerO = CreatePlayerFromArg(playerChoices, a1, "Player O");

            Console.WriteLine($"Player X {playerX.PlayerName} {playerX.AlgorithmName}");
            Console.WriteLine($"Player O {playerO.PlayerName} {playerO.AlgorithmName}");
        }
        else if (args.Length == 0)
        {
            // Interactive player selection
            playerX = SelectPlayer(playerChoices, "Player X");
            playerO = SelectPlayer(playerChoices, "Player O");
        }
        else if (args.Length != 2)
        {
            Console.WriteLine("Usage:");
            Console.WriteLine("  dotnet run                   - Interactive player selection");
            Console.WriteLine("  dotnet run <p1> <p2>        - Direct player selection"); 
            Console.WriteLine("  dotnet run --moves <moves>  - Load game from moves");
            Console.WriteLine();
            Console.WriteLine("Available algorithms:");
            foreach (var ((name, _), i) in playerChoices.Select((a, i) => (a, i + 1)))
            {
                Console.WriteLine($" - {i}: {name}");
            }
            Console.WriteLine();
            Console.WriteLine("Example moves: \"4 3 5 2 6 1\" (space-separated column numbers)");
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
            Console.WriteLine($" {i}: {name}");
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
