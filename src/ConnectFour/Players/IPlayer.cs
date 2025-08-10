namespace ConnectFour;

/// <summary>
/// Represents a Connect Four player.
/// </summary>
public interface IPlayer
{
    /// <summary>
    /// The display name of the player.
    /// </summary>
    string PlayerName { get; }

    /// <summary>
    /// descriptive name of the algoritm with its parameter. E.g. Negamax(4)
    /// </summary>
    string AlgorithmName { get; }

    /// <summary>
    /// Chooses a move for the given board and player state.
    /// </summary>
    /// <param name="board">The current game board.</param>
    /// <param name="player">The player making the move.</param>
    /// <returns>The column index (0-based) for the move.</returns>
    int ChooseMove(GameBoard board, CellState player);
}
