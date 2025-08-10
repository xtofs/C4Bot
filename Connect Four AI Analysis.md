# Connect Four AI Tournament Analysis

## Executive Summary

This analysis evaluates different AI algorithms for Connect Four through comprehensive tournament testing, with particular focus on the relationship between Negamax search depth and performance against random play.

## Tournament Setup

- **Games per match**: 200 (for statistical significance)
- **Players tested**: Random, Negamax (depths 3-8), MCTS-1000
- **Total games played**: 5,600 games across 28 pairwise matches

## Key Findings

### 1. Algorithm Performance Ranking

| Rank | Player | Win Rate | Key Characteristics |
|------|--------|----------|-------------------|
| 1 | MCTS-1000 | 95.1% | Simulation-based learning, strongest overall |
| 2 | Negamax-3 | 70.6% | Shallow search but surprisingly effective |
| 3 | Negamax-6 | 57.5% | Traditional depth, balanced performance |
| 4 | Negamax-8 | 44.8% | Deep search, perfect vs random but slower |
| 5 | Negamax-7 | 43.8% | High depth with diminishing returns |
| 6 | Negamax-4 | 43.3% | Standard depth implementation |
| 7 | Negamax-5 | 42.9% | Mid-range depth |
| 8 | Random | 2.0% | Baseline for comparison |

### 2. Negamax Depth vs Random Play Analysis

**Critical Discovery**: Strong negative correlation (-0.8386) between search depth and losses to random play.

| Depth | Random Wins | Loss Rate | Performance Gap |
|-------|-------------|-----------|----------------|
| 3 | 14/200 | 7.0% | Vulnerable to tactical accidents |
| 4 | 4/200 | 2.0% | Major improvement (-5%) |
| 5 | 4/200 | 2.0% | Plateau effect |
| 6 | 4/200 | 2.0% | Consistent with depth 4-5 |
| 7 | 2/200 | 1.0% | Incremental improvement |
| 8 | 0/200 | 0.0% | **Perfect performance** |

**Key Insight**: Depth 8 represents a categorical threshold where all tactical vulnerabilities to random play are eliminated.

### 3. Algorithm Comparison Insights

**MCTS Dominance:**
- MCTS-1000 defeats all Negamax variants decisively (86.5% - 98.5% win rates)
- Simulation-based approach superior to traditional tree search for Connect Four
- Perfect record against random play (100% win rate)

**Negamax Depth Paradox:**
- Negamax-3 outperforms deeper variants in overall tournament ranking
- Deeper search doesn't always translate to better overall performance
- Potential issues: evaluation function limitations, time constraints, or overfitting

**Head-to-Head Matrix Patterns:**
- Negamax variants show mostly 50-50 performance against each other (except depth 3)
- Depth 3 shows anomalous behavior: dominates some deeper variants completely
- Suggests potential implementation issues or algorithmic artifacts at certain depths

## Performance vs Computational Cost

| Algorithm | Avg Time/Game | Perfect vs Random | Overall Ranking |
|-----------|---------------|-------------------|-----------------|
| Negamax-3 | <1ms | No (93%) | 2nd |
| Negamax-6 | 3ms | No (98%) | 3rd |
| Negamax-8 | 59ms | Yes (100%) | 4th |
| MCTS-1000 | 11-59ms | Yes (100%) | 1st |

## Strategic Recommendations

### For Competitive Play:
- **Primary choice**: MCTS-1000 for maximum strength
- **Balanced option**: Negamax-6 for good performance with minimal computation
- **Fast option**: Negamax-3 for time-critical scenarios

### For Research:
- **Investigate Negamax-3 anomaly**: Why does shallow search outperform deeper variants?
- **Evaluation function analysis**: Current Negamax evaluation may be insufficient
- **MCTS parameter tuning**: Test different simulation counts for optimal performance

### For Educational Purposes:
- **Negamax-8**: Demonstrates perfect tactical play against random opponents
- **Correlation analysis**: Clear example of depth-performance relationships
- **Algorithm comparison**: Excellent case study for traditional vs modern AI approaches

## Technical Conclusions

1. **MCTS superiority**: Simulation-based learning significantly outperforms traditional minimax for Connect Four
2. **Depth threshold**: Negamax requires depth 8 to eliminate all tactical errors against random play
3. **Diminishing returns**: Beyond depth 4, additional Negamax depth provides minimal overall benefit
4. **Implementation quality**: The Negamax-3 anomaly suggests careful implementation verification is crucial

## Future Work

- Investigate Negamax evaluation function improvements
- Test MCTS with different simulation counts (500, 1500, 2000)
- Implement opening book integration
- Add time-constrained tournament analysis
- Explore hybrid MCTS-Negamax approaches
