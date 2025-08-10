using System;
using System.Collections.Generic;

namespace ConnectFour;

/// <summary>
/// A player that uses Monte Carlo Tree Search (MCTS) to select moves.
/// </summary>
public class MonteCarloTreeSearchPlayer : IPlayer
{
    /// <inheritdoc/>
    public string Name { get; }
    private readonly int simulations;

    /// <summary>
    /// Creates a new Monte Carlo Tree Search (MCTS) player with the given name and number of simulations.
    /// </summary>
    /// <param name="name">The display name for the player.</param>
    /// <param name="simulations">The number of simulations to run (default 1000).</param>
    public MonteCarloTreeSearchPlayer(string name, int simulations = 1000)
    {
        Name = name;
        this.simulations = simulations;
    }

    /// <inheritdoc/>
    public int ChooseMove(GameBoard board, CellState player)
    {
        var moves = board.GetAvailableMoves();
        var wins = new int[moves.Length];
        var plays = new int[moves.Length];
        var rnd = new Random();
        for (var i = 0; i < moves.Length; i++)
        {
            for (var j = 0; j < simulations / moves.Length; j++)
            {
                var simBoard = board.ApplyMove(moves[i], player);
                var winner = SimulateRandomGame(simBoard, Opponent(player), rnd);
                plays[i]++;
                if (winner == player)
                {
                    wins[i]++;
                }
            }
        }
        var best = 0;
        double bestRatio = -1;
        for (var i = 0; i < moves.Length; i++)
        {
            var ratio = plays[i] > 0 ? (double)wins[i] / plays[i] : 0;
            if (ratio > bestRatio)
            {
                bestRatio = ratio;
                best = i;
            }
        }
        return moves[best];
    }

    /// <summary>
    /// Simulates a random game from the given board state.
    /// </summary>
    private static CellState SimulateRandomGame(GameBoard board, CellState player, Random rnd)
    {
        while (true)
        {
            var result = board.GetGameResult();
            if (result == GameResult.WinX)
            {
                return CellState.X;
            }
            if (result == GameResult.WinO)
            {
                return CellState.O;
            }
            if (result == GameResult.Draw)
            {
                return CellState.Empty;
            }

            var moves = board.GetAvailableMoves();
            if (moves.Length == 0)
            {
                return CellState.Empty;
            }

            var move = moves[rnd.Next(moves.Length)];
            board = board.ApplyMove(move, player);
            player = Opponent(player);
        }
    }

    private static CellState Opponent(CellState player) => player == CellState.X ? CellState.O : CellState.X;
}
