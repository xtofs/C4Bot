using System;

namespace ConnectFour;

/// <summary>
/// A player that prompts a human for input via the console.
/// </summary>
public class HumanPlayer : IPlayer
{
    /// <inheritdoc/>
    public string Name { get; }

    /// <summary>
    /// Creates a new human player with the given name.
    /// </summary>
    /// <param name="name">The display name for the player.</param>
    public HumanPlayer(string name) => Name = name;

    /// <inheritdoc/>
    public int ChooseMove(GameBoard board, CellState player)
    {
        while (true)
        {
            Console.Write($"{Name}, enter column (1-{GameBoard.Columns}): ");
            if (int.TryParse(Console.ReadLine(), out int col) && col >= 1 && col <= GameBoard.Columns && !board.IsColumnFull(col - 1))
                return col - 1;
            Console.WriteLine("Invalid move. Try again.");
        }
    }
}
