# Connect Four AI Player Ideas

This document outlines various approaches for creating new automated players based on our tournament analysis and research into Connect Four AI strategies.

## Current State Analysis

Based on our tournament results, we identified several key insights:

- **MCTS dominance**: 95.3% overall win rate, excels at positional understanding
- **Negamax-3 anomaly**: 61.1% win rate despite shallow depth suggests evaluation function limitations
- **Depth threshold**: Strong correlation (-0.92) between search depth and tactical robustness
- **Performance gap**: Significant opportunity between Negamax variants and MCTS

## Proposed AI Player Approaches

### 1. Enhanced Negamax with Static Evaluation

**Concept**: The current Negamax only evaluates terminal positions (win/loss/draw = ±10000/0). Adding positional evaluation could dramatically improve performance.

**Implementation**:

```csharp
public class EvaluatedNegamaxPlayer : IPlayer
{
    private static int EvaluatePosition(GameBoard board, CellState player)
    {
        // Center column preference (Connect Four strategy)
        int score = board.GetPieceCount(3, player) * 3;
        
        // Evaluate potential four-in-a-rows
        score += CountThreats(board, player) * 50;
        score -= CountThreats(board, Opponent(player)) * 60; // Defend > Attack
        
        // Control of center columns
        for (int col = 2; col <= 4; col++)
            score += board.GetPieceCount(col, player) * 2;
            
        return score;
    }
}
```

**Rationale**: Our Negamax-3 anomaly suggests the evaluation function is the bottleneck, not search depth. Connect Four has well-known positional principles that could be encoded.

**Evaluation Components**:

- **Center control** (columns 3-4 are most valuable)
- **Threat detection** (count 3-in-a-rows)
- **Blocking priorities** (defend before attack)
- **Vertical stacking** (build upward pressure)

### 2. Hybrid Opening/Endgame Algorithm

**Concept**: Use different algorithms for different game phases, leveraging the strengths of each approach.

**Implementation**:

```csharp
public class HybridPlayer : IPlayer
{
    private readonly IPlayer openingPlayer;   // Fast, positional
    private readonly IPlayer endgamePlayer;   // Deep, tactical
    
    public int ChooseMove(GameBoard board, CellState player)
    {
        int moveCount = CountTotalMoves(board);
        
        // Opening: Use MCTS for positional understanding
        if (moveCount < 14) 
            return openingPlayer.ChooseMove(board, player);
            
        // Endgame: Use deep Negamax for tactical precision
        return endgamePlayer.ChooseMove(board, player);
    }
}
```

**Rationale**: MCTS excels at positional play and long-term planning, while deep Negamax (depth 8) is tactically perfect. Combining both could leverage their respective strengths.

**Phase Definitions**:

- **Opening** (moves 1-14): Positional foundation, pattern recognition
- **Endgame** (moves 15+): Tactical calculation, threat resolution

### 3. Ensemble/Committee Algorithm

**Concept**: Run multiple algorithms and use voting or confidence metrics to select the best move.

**Implementation**:

```csharp
public class EnsemblePlayer : IPlayer
{
    private readonly List<IPlayer> players;
    
    public int ChooseMove(GameBoard board, CellState player)
    {
        var votes = new Dictionary<int, int>();
        var confidences = new Dictionary<int, double>();
        
        // Get moves from all players
        foreach (var p in players)
        {
            var move = p.ChooseMove(board, player);
            votes[move] = votes.GetValueOrDefault(move, 0) + 1;
            
            // Add confidence weighting based on player type
            var weight = GetPlayerWeight(p, board);
            confidences[move] = confidences.GetValueOrDefault(move, 0) + weight;
        }
        
        // Choose move with highest confidence-weighted votes
        return confidences.OrderByDescending(kvp => kvp.Value).First().Key;
    }
}
```

**Advantages**:

- **Robustness**: Reduces individual algorithm weaknesses
- **Adaptability**: Different algorithms can handle different position types
- **Consensus**: Multiple perspectives on position evaluation

**Weighting Strategies**:

- Time-based (favor faster algorithms under time pressure)
- Position-based (favor tactical vs positional specialists)
- Historical performance-based

### 4. Iterative Deepening with Time Management

**Concept**: Adaptive depth based on position complexity and available computation time.

**Implementation**:

```csharp
public class AdaptiveNegamaxPlayer : IPlayer
{
    public int ChooseMove(GameBoard board, CellState player)
    {
        var availableMoves = board.GetAvailableMoves();
        
        // Simple positions: shallow search
        if (availableMoves.Length >= 5)
            return new NegamaxPlayer("Fast", 4).ChooseMove(board, player);
            
        // Complex positions: deeper search
        if (HasImmediateThreat(board))
            return new NegamaxPlayer("Deep", 8).ChooseMove(board, player);
            
        // Default
        return new NegamaxPlayer("Standard", 6).ChooseMove(board, player);
    }
}
```

**Complexity Indicators**:

- **Move count**: Fewer available moves = more critical
- **Threat density**: Multiple threats require deeper analysis
- **Material balance**: Endgame positions need precise calculation

### 5. Pattern Recognition Player

**Concept**: A pattern-recognition approach using position encoding and learned responses.

**Implementation**:

```csharp
public class PatternPlayer : IPlayer
{
    private readonly Dictionary<ulong, int> patterns = new();
    
    public int ChooseMove(GameBoard board, CellState player)
    {
        var boardHash = board.GetBitboard(); // Use existing bitboard
        
        // Look for learned patterns
        if (patterns.ContainsKey(boardHash))
            return patterns[boardHash];
            
        // Fall back to heuristic for unknown positions
        return ChooseBestHeuristic(board, player);
    }
    
    // Could be trained by observing MCTS games
}
```

**Learning Sources**:

- **Self-play**: Generate training data from strong players
- **Expert games**: Learn from human expert play
- **MCTS analysis**: Extract patterns from successful MCTS decisions

### 6. Neural Network Player

**Concept**: Deep learning approach with position evaluation and move prediction.

**Architecture**:

- **Input**: Board state as 6x7x3 tensor (empty/player1/player2)
- **Hidden**: Convolutional layers for pattern recognition
- **Output**: Move probabilities + position evaluation

**Training Data**: Could be generated from MCTS self-play games.

## Implementation Priority

Based on our analysis, the recommended implementation order:

1. **Enhanced Negamax** - Most likely to show immediate improvement
2. **Hybrid Opening/Endgame** - Leverages known algorithm strengths
3. **Adaptive Negamax** - Builds on Enhanced Negamax insights
4. **Ensemble Player** - Combines multiple working algorithms
5. **Pattern Player** - Research-oriented, requires training data

## Research Questions

Each approach addresses different research questions:

- **Enhanced Negamax**: Can better evaluation bridge the MCTS gap?
- **Hybrid**: Can algorithm specialization outperform generalists?
- **Ensemble**: Do multiple perspectives improve decision quality?
- **Adaptive**: Can dynamic complexity assessment optimize resources?
- **Pattern**: Can learned patterns compete with search algorithms?

## Success Metrics

- **Performance vs existing players**: Win rate improvements
- **Computational efficiency**: Time per move
- **Tactical robustness**: Performance against Random player
- **Consistency**: Variance in performance across multiple runs
