namespace ConnectFour.Players;

/// <summary>
/// Interface for position evaluation strategies.
/// </summary>
public interface IPositionEvaluator
{
    /// <summary>
    /// Evaluates a game position and returns a score from the specified player's perspective.
    /// </summary>
    /// <param name="board">The game board to evaluate.</param>
    /// <param name="player">The player from whose perspective to evaluate.</param>
    /// <returns>The evaluation score (positive favors the player, negative favors opponent).</returns>
    int Evaluate(GameBoard board, CellState player);
}
