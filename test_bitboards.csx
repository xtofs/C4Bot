#r "src/ConnectFour/bin/Debug/net8.0/ConnectFour.dll"

using ConnectFour;

// Create a simple board with a few moves
var board = GameBoard.FromMoves("4 3 5 2");

Console.WriteLine("Game Board:");
var grid = board.ToArray();
for (int row = 5; row >= 0; row--)
{
    for (int col = 0; col < 7; col++)
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

// Show debug bitboards
Console.WriteLine(board.ToDebugBitboards());
