using System;

namespace ConnectFour;

/// <summary>
/// A player that prompts a human for input via the console.
/// </summary>
/// <remarks>
/// Creates a new human player with the given name.
/// </remarks>
/// <param name="name">The display name for the player.</param>
public class InteractivePlayer(string name) : IPlayer
{
    /// <inheritdoc/>
    public string PlayerName { get; } = name;

    public string AlgorithmName => "Interactive";

    /// <inheritdoc/>
    public int ChooseMove(GameBoard board, CellState player)
    {
        while (true)
        {
            Console.Write($"{PlayerName} ({(player == CellState.X ? "X" : "O")}), enter column (1-{GameBoard.Columns}): ");
            if (int.TryParse(Console.ReadLine(), out var col) && col >= 1 && col <= GameBoard.Columns && !board.IsColumnFull(col - 1))
            {
                return col - 1;
            }

            Console.WriteLine("Invalid move. Try again.");
        }
    }
}
