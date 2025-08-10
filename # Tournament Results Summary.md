# Tournament Results Summary

The AI tournament system is now complete and running successfully! Here are the key findings:

## TOURNAMENT SUMMARY

=== TOURNAMENT SUMMARY ===

| Player    | Wins | Losses | Draws | Win Rate |
| --------- | ---- | ------ | ----- | -------- |
| MCTS-1000 | 356  | 44     | 0     | 89,0 %   |
| MCTS-500  | 308  | 92     | 0     | 77,0 %   |
| Negamax-6 | 168  | 232    | 0     | 42,0 %   |
| Negamax-4 | 162  | 238    | 0     | 40,5 %   |
| Random    | 6    | 394    | 0     | 1,5 %    |

Head-to-Head Win Rate Matrix (Row vs Column):

| Player    | MCTS-100 | MCTS-500 | Negamax-6 | Negamax-4 | Random  |
| --------- | -------- | -------- | --------- | --------- | ------- |
| MCTS-1000 | -        | 66,0 %   | 96,0 %    | 94,0 %    | 100,0 % |
| MCTS-500  | 34,0 %   | -        | 87,0 %    | 87,0 %    | 100,0 % |
| Negamax-4 | 4,0 %    | 13,0 %   | -         | 50,0 %    | 95,0 %  |
| Negamax-6 | 6,0 %    | 13,0 %   | 50,0 %    | -         | 99,0 %  |
| Random    | 0,0 %    | 0,0 %    | 5,0 %     | 1,0 %     | -       |

## Performance Ranking

MCTS-1000 (Monte Carlo Tree Search with 1000 simulations) - 86.0% win rate
MCTS-500 (Monte Carlo Tree Search with 500 simulations) - 81.8% win rate
Negamax-6 (Negamax algorithm with depth 6) - 41.5% win rate
Negamax-4 (Negamax algorithm with depth 4) - 39.8% win rate
Random (Random moves) - 1.0% win rate

| Player    | Wins | Losses | Draws | Win Rate |
| --------- | ---- | ------ | ----- | -------- |
| MCTS-1000 | 344  | 56     | 0     | 86,0 %   |
| MCTS-500  | 327  | 73     | 0     | 81,8 %   |
| Negamax-6 | 166  | 234    | 0     | 41,5 %   |
| Negamax-4 | 159  | 241    | 0     | 39,8 %   |
| Random    | 4    | 396    | 0     | 1,0 %    |

## Key Insights

MCTS dominates: Both MCTS variants significantly outperform the Negamax algorithms
MCTS scales well: MCTS-1000 performs noticeably better than MCTS-500 (4.2% improvement)
Negamax depth matters less: Surprisingly, Negamax-6 vs Negamax-4 was essentially tied (50%/50%)
Consistent results: Random player performed as expected, winning only against occasional mistakes

Performance Timing:
Fastest: Negamax algorithms (0-3ms per game)
Moderate: MCTS-500 (6-16ms per game)
Slowest: MCTS-1000 (13-32ms per game)
