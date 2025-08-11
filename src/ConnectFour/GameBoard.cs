namespace ConnectFour;

using System.Diagnostics.CodeAnalysis;
using System.Numerics;

public sealed class GameBoard
{
    public const int Rows = 6;
    public const int Columns = 7;
    private const int BitsPerColumn = Rows + 1;
    private const ulong BoardMask = 0x1FFFFFFFFFFFFUL;
    private const ulong TopRowMask = 0x40810204081UL; // Top row of each column

    /// <summary>
    /// Gets the bitboard for player X (for advanced position evaluation).
    /// 
    /// DESIGN NOTE: This property exposes internal implementation details, which normally
    /// violates encapsulation. However, this is a deliberate compromise to allow performance-critical
    /// code (like BitboardPositionEvaluator) direct access to efficient bitboard operations.
    /// For typical use cases, prefer ToArray() or the indexer for better encapsulation.
    /// 
    /// Be aware that this is an implementation detail and should be used with caution.
    /// The bit layout is: 
    ///     bit number i represents the grid cell (col,row) where 
    ///     i = col * 7 + row,
    ///     col = i / 7, 
    ///     row = i % 7.
    /// Each column uses 7 bits (rows 0-5 for the board, bit 6 is unused).
    /// Bit positions 0-6 are column 0, bits 7-13 are column 1, etc.
    /// For example: bit 0 = (0,0), bit 1 = (0,1), bit 7 = (1,0), bit 14 = (2,0).
    /// </summary>    
    public ulong XBitboard { get; }

    /// <summary>
    /// Gets the bitboard for player O (for advanced position evaluation).
    /// 
    /// DESIGN NOTE: This property exposes internal implementation details, which normally
    /// violates encapsulation. However, this is a deliberate compromise to allow performance-critical
    /// code (like BitboardPositionEvaluator) direct access to efficient bitboard operations.
    /// For typical use cases, prefer ToArray() or the indexer for better encapsulation.
    /// 
    /// Be aware that this is an implementation detail and should be used with caution.
    /// The bit layout is: 
    ///     bit number i represents the grid cell (col,row) where 
    ///     i = col * 7 + row,
    ///     col = i / 7, 
    ///     row = i % 7.
    /// Each column uses 7 bits (rows 0-5 for the board, bit 6 is unused).
    /// Bit positions 0-6 are column 0, bits 7-13 are column 1, etc.
    /// For example: bit 0 = (0,0), bit 1 = (0,1), bit 7 = (1,0), bit 14 = (2,0).
    /// </summary>    
    public ulong OBitboard { get; }

    public GameBoard() : this(0, 0)
    {
    }

    internal GameBoard(ulong x, ulong o)
    {
        XBitboard = x;
        OBitboard = o;
    }

    /// <summary>
    /// Returns the result of the game (WinX, WinO, Draw, or Ongoing) efficiently, without calculating winning cells.
    /// </summary>
    /// <returns>The result of the game.</returns>
    public GameResult GetGameResult()
    {
        if (IsWin(CellState.X)) { return GameResult.WinX; }
        if (IsWin(CellState.O)) { return GameResult.WinO; }
        if (IsDraw()) { return GameResult.Draw; }
        return GameResult.Ongoing;
    }

    [Obsolete("this is a fairly expensive operation that should be replaced with operations that leverage the bitboards")]
    public CellState this[int row, int col]
    {
        get
        {
            var pos = col * BitsPerColumn + row;
            if (((XBitboard >> pos) & 1UL) != 0)
            {
                return CellState.X;
            }

            if (((OBitboard >> pos) & 1UL) != 0)
            {
                return CellState.O;
            }

            return CellState.Empty;
        }
    }

    public bool IsColumnFull(int col)
    {
        return ((XBitboard | OBitboard) & (1UL << (col * BitsPerColumn + Rows - 1))) != 0;
    }

    /// <summary>
    /// Apply a move to the board, returning a new board with the move applied.
    /// </summary>
    /// <param name="col">The column to play in (0-based).</param>
    /// <param name="player">The player making the move (must be X or O).</param>
    /// <returns>A new GameBoard with the move applied.</returns>
    /// <exception cref="InvalidOperationException">Thrown if the column is full or player is Empty.</exception>
    public GameBoard ApplyMove(int col, CellState player)
    {
        return ApplyMove(col, player, out _);
    }

    /// <summary>
    /// Apply a move to the board, returning a new board with the move applied and the position where the piece was placed.
    /// </summary>
    /// <param name="col">The column to play in (0-based).</param>
    /// <param name="player">The player making the move (must be X or O).</param>
    /// <param name="placedAt">The coordinates (row, col) where the piece was placed.</param>
    /// <returns>A new GameBoard with the move applied.</returns>
    /// <exception cref="InvalidOperationException">Thrown if the column is full or player is Empty.</exception>
    public GameBoard ApplyMove(int col, CellState player, out (int row, int col) placedAt)
    {
        var mask = XBitboard | OBitboard;
        var colMask = ((1UL << Rows) - 1) << (col * BitsPerColumn);
        var filled = mask & colMask;
        var height = BitOperations.PopCount(filled);

        if (height >= Rows)
        {
            throw new InvalidOperationException("Column is full");
        }

        var newBit = 1UL << (col * BitsPerColumn + height);
        placedAt = (height, col);
        return player == CellState.X
            ? new GameBoard(XBitboard | newBit, OBitboard)
            : new GameBoard(XBitboard, OBitboard | newBit);
    }

    private bool IsWin(CellState player)
    {
        var board = player == CellState.X ? XBitboard : OBitboard;
        var m = board & (board >> BitsPerColumn);
        if ((m & (m >> (2 * BitsPerColumn))) != 0)
        {
            return true;
        }

        m = board & (board >> 1);
        if ((m & (m >> 2)) != 0)
        {
            return true;
        }

        m = board & (board >> (BitsPerColumn - 1));
        if ((m & (m >> (2 * (BitsPerColumn - 1)))) != 0)
        {
            return true;
        }

        m = board & (board >> (BitsPerColumn + 1));
        if ((m & (m >> (2 * (BitsPerColumn + 1)))) != 0)
        {
            return true;
        }

        return false;
    }

    private bool IsDraw()
    {
        return ((XBitboard | OBitboard) & BoardMask) == BoardMask;
    }

    public int[] GetAvailableMoves()
    {
        var moves = new List<int>();
        for (var col = 0; col < Columns; col++)
        {
            if (!IsColumnFull(col))
            {
                moves.Add(col);
            }
        }

        return moves.ToArray();
    }

    public void GetAvailableMoves(Span<int> moves, out int count)
    {
        count = 0;
        ulong topRow = (XBitboard | OBitboard) & TopRowMask;  // Check which columns are full
        for (int col = 0; col < Columns; col++)
        {
            if ((topRow & (1UL << (col * BitsPerColumn + Rows - 1))) == 0)
            {
                moves[count++] = col;
            }
        }
    }

    /// <summary>
    /// Gets the total number of moves (pieces) that have been played on the board.
    /// Each move by either player counts as one half-move in chess terminology.
    /// </summary>
    public int HalfMoveCount => BitOperations.PopCount(XBitboard | OBitboard);

    /// <summary>
    /// Checks if the game has ended (win or draw) and provides the result and winning cells if applicable.
    /// Returns true if the game has ended (win or draw), false if ongoing.
    /// If true, <paramref name="result"/> is WinX, WinO, or Draw.
    /// If <paramref name="result"/> is WinX or WinO, <paramref name="winningCells"/> contains the four-in-a-row cells; if Draw, <paramref name="winningCells"/> is null.
    /// If false, <paramref name="result"/> is Ongoing and <paramref name="winningCells"/> is null.
    /// </summary>
    /// <param name="result">The result of the game (WinX, WinO, Draw, or Ongoing).</param>
    /// <param name="winningCells">The set of winning cells if there is a win, otherwise null.</param>
    /// <returns>True if the game has ended, false if ongoing.</returns>
    public bool HasGameEnded([MaybeNullWhen(true)] out GameResult result, [MaybeNullWhen(true)] out HashSet<(int row, int col)>? winningCells)
    {
        // Fast win check for X
        if (IsWin(CellState.X))
        {
            result = GameResult.WinX;
            winningCells = GetWinningCells(CellState.X);
            return true;
        }
        // Fast win check for O
        if (IsWin(CellState.O))
        {
            result = GameResult.WinO;
            winningCells = GetWinningCells(CellState.O);
            return true;
        }
        // Check draw
        if (((XBitboard | OBitboard) & BoardMask) == BoardMask)
        {
            result = GameResult.Draw;
            winningCells = null;
            return true;
        }
        result = GameResult.Ongoing;
        winningCells = null;
        return false;
    }

    /// <summary>
    /// Returns the set of (row, col) for the winning four-in-a-row for the given player, or empty if none.
    /// </summary>
    /// <param name="player">The player to check for a win (CellState.X or CellState.O).</param>
    /// <returns>A set of (row, col) tuples for the winning four, or empty if no win.</returns>
    public HashSet<(int row, int col)> GetWinningCells(CellState player)
    {
        var board = player == CellState.X ? XBitboard : OBitboard;

        // Check horizontal wins (4 in a row across columns)
        var horizontal = board & (board >> BitsPerColumn) & (board >> (2 * BitsPerColumn)) & (board >> (3 * BitsPerColumn));
        if (horizontal != 0)
        {
            var (row, col) = GetFirstSetBit(horizontal);
            return [(row, col), (row, col + 1), (row, col + 2), (row, col + 3)];
        }

        // Check vertical wins (4 in a row down rows)
        var vertical = board & (board >> 1) & (board >> 2) & (board >> 3);
        if (vertical != 0)
        {
            var (row, col) = GetFirstSetBit(vertical);
            return [(row, col), (row + 1, col), (row + 2, col), (row + 3, col)];
        }

        // Check diagonal wins (\) - top-left to bottom-right (up-left direction)
        var diagonal1 = board & (board >> (BitsPerColumn - 1)) & (board >> (2 * (BitsPerColumn - 1))) & (board >> (3 * (BitsPerColumn - 1)));
        if (diagonal1 != 0)
        {
            var (row, col) = GetFirstSetBit(diagonal1);
            return [(row, col), (row + 1, col - 1), (row + 2, col - 2), (row + 3, col - 3)];
        }

        // Check diagonal wins (/) - bottom-left to top-right (up-right direction)
        var diagonal2 = board & (board >> (BitsPerColumn + 1)) & (board >> (2 * (BitsPerColumn + 1))) & (board >> (3 * (BitsPerColumn + 1)));
        if (diagonal2 != 0)
        {
            var (row, col) = GetFirstSetBit(diagonal2);
            return [(row, col), (row + 1, col + 1), (row + 2, col + 2), (row + 3, col + 3)];
        }

        return [];
    }

    /// <summary>
    /// Finds the first set bit in the bitboard and converts it to (row, col) coordinates.
    /// </summary>
    private static (int row, int col) GetFirstSetBit(ulong bitboard)
    {
        int bitPosition = BitOperations.TrailingZeroCount(bitboard);
        int col = bitPosition / BitsPerColumn;
        int row = bitPosition % BitsPerColumn;
        return (row, col);
    }

    /// <summary>
    /// Returns the current board state as a 2D array for display purposes.
    /// This provides a clean abstraction that doesn't expose bitboard internals.
    /// </summary>
    /// <returns>A 2D array where [row, col] contains the CellState at that position.</returns>
    public CellState[,] ToArray()
    {
        var grid = new CellState[Rows, Columns];
        
        for (int row = 0; row < Rows; row++)
        {
            for (int col = 0; col < Columns; col++)
            {
                ulong position = 1UL << (col * BitsPerColumn + row);
                if ((XBitboard & position) != 0)
                    grid[row, col] = CellState.X;
                else if ((OBitboard & position) != 0)
                    grid[row, col] = CellState.O;
                else
                    grid[row, col] = CellState.Empty;
            }
        }
        
        return grid;
    }

    /// <summary>
    /// Creates a GameBoard by replaying a sequence of moves.
    /// </summary>
    /// <param name="moves">Move sequence like "4 3 5 2 6 1" (space-separated column numbers)</param>
    /// <returns>GameBoard after all moves have been applied</returns>
    public static GameBoard FromMoves(string moves)
    {
        if (string.IsNullOrWhiteSpace(moves))
            return new GameBoard();

        var moveList = moves.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        var board = new GameBoard();
        
        for (int i = 0; i < moveList.Length; i++)
        {
            if (int.TryParse(moveList[i].Trim(), out int col) && col >= 1 && col <= Columns)
            {
                var player = i % 2 == 0 ? CellState.X : CellState.O;
                board = board.ApplyMove(col - 1, player); // Convert to 0-based
            }
            else
            {
                throw new ArgumentException($"Invalid move '{moveList[i]}' in moves. Moves must be column numbers 1-{Columns}.");
            }
        }
        
        return board;
    }

    /// <summary>
    /// Generates a moves string from a sequence of moves.
    /// </summary>
    /// <param name="moves">Sequence of 0-based column numbers</param>
    /// <returns>Moves string like "4 3 5 2 6 1"</returns>
    public static string ToMoves(IEnumerable<int> moves)
    {
        return string.Join(" ", moves.Select(col => (col + 1).ToString()));
    }
}
