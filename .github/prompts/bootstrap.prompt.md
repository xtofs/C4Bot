---
mode: agent
---
implement  Connect Four as a console application with human and computer player

First computer player should use negamax (or Minimax) with Alpha-Beta pruning

Second computer player should use Monte Carlo Tree Search (MCTS) for its decision-making.

human and computer player classes should use a common interface to be pluged in into a common "run one game round" framework.

The board state implementation should have a ergonomic external interface (cell testing, move application, ) but its implementation should use 64 bit bitboards with efficient compiler intrinsics or bit fiddling for move generation, win testing, win configuration extranction, ...

The implementation should prefer to use immutable types and reduce allocation where sensible.

implementation should follow modern C# idioms and best practices and specifically adhere to D:\.editorconfig