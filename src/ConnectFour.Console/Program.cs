using ConnectFour;

namespace ConnectFour.ConsoleApp;

class Program
{
    static void Main(string[] args)
    {
        System.Console.WriteLine("Welcome to Connect Four!");
        IPlayer playerX, playerO;
        if (args.Length >= 2 && IsValidArg(args[0]) && IsValidArg(args[1]))
        {
            playerX = CreatePlayerFromArg(args[0], "Player X");
            playerO = CreatePlayerFromArg(args[1], "Player O");
            System.Console.WriteLine($"Player X: {playerX.GetType().Name}");
            System.Console.WriteLine($"Player O: {playerO.GetType().Name}");
        }
        else
        {
            System.Console.WriteLine("Select Player X: 1=Human, 2=Negamax, 3=MCTS, 4=Random");
            playerX = SelectPlayerX();
            System.Console.WriteLine("Select Player O: 1=Human, 2=Negamax, 3=MCTS, 4=Random");
            playerO = SelectPlayerO();
        }
        GameRunner.RunGame(playerX, playerO);
    }

    static bool IsValidArg(string arg)
    {
        return arg == "1" || arg == "2" || arg == "3" || arg == "4";
    }

    static IPlayer CreatePlayerFromArg(string arg, string name)
    {
        return arg switch
        {
            "1" => new HumanPlayer(name),
            "2" => new NegamaxPlayer(name),
            "3" => new MonteCarloTreeSearchPlayer(name),
            "4" => new RandomPlayer(name),
            _ => throw new ArgumentException($"Invalid player argument: {arg}")
        };
    }

    static IPlayer SelectPlayerX()
    {
        while (true)
        {
            var input = System.Console.ReadLine();
            switch (input)
            {
                case "1": return new HumanPlayer("Player X");
                case "2": return new NegamaxPlayer("Player X");
                case "3": return new MonteCarloTreeSearchPlayer("Player X");
                case "4": return new RandomPlayer("Player X");
            }
            System.Console.WriteLine("Invalid selection. Enter 1, 2, 3, or 4:");
        }
    }

    static IPlayer SelectPlayerO()
    {
        while (true)
        {
            var input = System.Console.ReadLine();
            switch (input)
            {
                case "1": return new HumanPlayer("Player O");
                case "2": return new NegamaxPlayer("Player O");
                case "3": return new MonteCarloTreeSearchPlayer("Player O");
                case "4": return new RandomPlayer("Player O");
            }
            System.Console.WriteLine("Invalid selection. Enter 1, 2, 3, or 4:");
        }
    }
}
