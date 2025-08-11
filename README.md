# Connect Four console game

## About This Project

This repository contains a modular Connect Four AI engine and tournament system, supporting multiple algorithms:

- Monte Carlo Tree Search (MCTS)
- Minimax (with variable depth)
- Random player (baseline)

### Features

- Pluggable AI player architecture
- Tournament automation and statistics
- Type-safe enum design for clarity and correctness
- Console interface for interactive play and analysis

### Getting Started

1. Clone the repository
2. Build with .NET 8.0+
3. Run tournaments or play interactively via the console project

---

## Connect Four AI Tournament & Engine

### Tournament Results Summary

| Player                | Wins | Losses | Draws | Win Rate |
|-----------------------|------|--------|-------|----------|
| MonteCarlo-1000       | 299  | 101    | 0     | 74.8%    |
| Hybrid-MCTS/Enhanced  | 286  | 114    | 0     | 71.5%    |
| EnhancedMinimax-6     | 251  | 149    | 0     | 62.7%    |
| Minimax-6             | 163  | 237    | 0     | 40.7%    |
| Random                | 1    | 399    | 0     | 0.3%     |

#### Head-to-Head Win Rate Matrix (Row vs Column)

| Player              | Enhanced | Hybrid-M | Minimax-6 | MonteCarlo | Random  |
|---------------------|----------|----------|-----------|------------|---------|
| EnhancedMinimax-6   | -        | 54.0%    | 50.0%     | 47.0%      | 100.0%  |
| Hybrid-MCTS/Enhanced| 46.0%    | -        | 91.0%     | 49.0%      | 100.0%  |
| Minimax-6           | 50.0%    | 9.0%     | -         | 5.0%       | 99.0%   |
| MonteCarlo-1000     | 53.0%    | 51.0%    | 95.0%     | -          | 100.0%  |
| Random              | 0.0%     | 0.0%     | 1.0%      | 0.0%       | -       |

#### Performance Metrics

| Player                | Games | Total Time | Avg/Game |
|-----------------------|-------|------------|----------|
| Random                | 400   | 5.8s       | 14ms     |
| Minimax-6             | 400   | 12.6s      | 32ms     |
| EnhancedMinimax-6     | 400   | 14.3s      | 36ms     |
| Hybrid-MCTS/Enhanced  | 400   | 18.0s      | 45ms     |
| MonteCarlo-1000       | 400   | 21.5s      | 54ms     |

---
