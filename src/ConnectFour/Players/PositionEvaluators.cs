namespace ConnectFour.Players;

using System.Numerics;

/// <summary>
/// Basic evaluator that provides no static evaluation of positions.
/// Always returns 0, relying solely on the search tree to find terminal positions.
/// </summary>
public class BasicPositionEvaluator : IPositionEvaluator
{
    public int Evaluate(GameBoard board, CellState player)
    {
        return 0; // No static evaluation - purely tree-search based
    }
}

/// <summary>
/// High-performance bitboard-based position evaluator for Connect Four.
/// Uses bitwise operations instead of expensive cell-by-cell board scanning.
/// Provides Connect Four-specific heuristics optimized for speed.
/// </summary>
public class BitboardPositionEvaluator : IPositionEvaluator
{
    // Precomputed column masks for center control evaluation
    private const ulong Column3Mask = 0x1FC0000UL;  // Column 3 (center-left)
    private const ulong Column4Mask = 0x3F800000UL; // Column 4 (center-right)  
    private const ulong CenterColumnsMask = Column3Mask | Column4Mask;
    
    public int Evaluate(GameBoard board, CellState player)
    {
        var playerBits = player == CellState.X ? board.XBitboard : board.OBitboard;
        var opponentBits = player == CellState.X ? board.OBitboard : board.XBitboard;
        
        int score = 0;
        
        // Center control - count pieces in center columns (simplified but fast)
        score += BitOperations.PopCount(playerBits & CenterColumnsMask) * 3;
        score -= BitOperations.PopCount(opponentBits & CenterColumnsMask) * 3;
        
        // Threat evaluation - detect 3-in-a-row patterns
        score += CountThreats(playerBits, opponentBits) * 50;
        score -= CountThreats(opponentBits, playerBits) * 60; // Defense slightly more important
        
        return score;
    }
    
    /// <summary>
    /// Fast bitboard-based threat detection for 3-in-a-row patterns.
    /// Uses bit-shifting to detect horizontal, vertical, and diagonal patterns.
    /// </summary>
    private static int CountThreats(ulong playerBits, ulong opponentBits)
    {
        const int BitsPerColumn = 7; // 6 rows + 1 separator
        int threats = 0;
        
        // Horizontal threats: check for XXX_ patterns
        var horizontal = playerBits & (playerBits >> BitsPerColumn) & (playerBits >> (2 * BitsPerColumn));
        // Check if the 4th position is empty (not occupied by either player)
        var emptySpaces = ~(playerBits | opponentBits);
        threats += BitOperations.PopCount(horizontal & (emptySpaces >> (3 * BitsPerColumn)));
        threats += BitOperations.PopCount(horizontal & (emptySpaces << (3 * BitsPerColumn)));
        
        // Vertical threats: check for XXX_ patterns (pieces stacked vertically)
        var vertical = playerBits & (playerBits >> 1) & (playerBits >> 2);
        threats += BitOperations.PopCount(vertical & (emptySpaces >> 3));
        
        // Diagonal threats (/) - shift by (BitsPerColumn - 1)
        var diagonal1 = playerBits & (playerBits >> (BitsPerColumn - 1)) & (playerBits >> (2 * (BitsPerColumn - 1)));
        threats += BitOperations.PopCount(diagonal1 & (emptySpaces >> (3 * (BitsPerColumn - 1))));
        threats += BitOperations.PopCount(diagonal1 & (emptySpaces << (3 * (BitsPerColumn - 1))));
        
        // Diagonal threats (\) - shift by (BitsPerColumn + 1)  
        var diagonal2 = playerBits & (playerBits >> (BitsPerColumn + 1)) & (playerBits >> (2 * (BitsPerColumn + 1)));
        threats += BitOperations.PopCount(diagonal2 & (emptySpaces >> (3 * (BitsPerColumn + 1))));
        threats += BitOperations.PopCount(diagonal2 & (emptySpaces << (3 * (BitsPerColumn + 1))));
        
        return threats;
    }
}
