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

    private readonly ulong x;
    private readonly ulong o;

    public GameBoard() : this(0, 0)
    {
    }

    internal GameBoard(ulong x, ulong o)
    {
        this.x = x;
        this.o = o;
    }

    /// <summary>
    /// Returns the result of the game (WinX, WinO, Draw, or Ongoing) efficiently, without calculating winning cells.
    /// </summary>
    /// <returns>The result of the game.</returns>
    public GameResult GetGameResult()
    {
        if (IsWin(CellState.X)) return GameResult.WinX;
        if (IsWin(CellState.O)) return GameResult.WinO;
        if (IsDraw()) return GameResult.Draw;
        return GameResult.Ongoing;
    }

    public CellState this[int row, int col]
    {
        get
        {
            var pos = col * BitsPerColumn + row;
            if (((x >> pos) & 1UL) != 0)
            {
                return CellState.X;
            }

            if (((o >> pos) & 1UL) != 0)
            {
                return CellState.O;
            }

            return CellState.Empty;
        }
    }

    public bool IsColumnFull(int col)
    {
        return ((x | o) & (1UL << (col * BitsPerColumn + Rows - 1))) != 0;
    }

    /// <summary>
    /// Returns a new GameBoard with the specified move applied for the given player.
    /// Throws if the column is full or if player is CellState.Empty.
    /// </summary>
    /// <param name="col">The column to play in (0-based).</param>
    /// <param name="player">The player making the move (must be X or O).</param>
    /// <returns>A new GameBoard with the move applied.</returns>
    /// <exception cref="InvalidOperationException">Thrown if the column is full or player is Empty.</exception>
    public GameBoard ApplyMove(int col, CellState player)
    {
        var mask = x | o;
        var colMask = ((1UL << Rows) - 1) << (col * BitsPerColumn);
        var filled = mask & colMask;
        var height = BitOperations.PopCount(filled);
        
        if (height >= Rows) throw new InvalidOperationException("Column is full");
        
        var newBit = 1UL << (col * BitsPerColumn + height);
        return player == CellState.X 
            ? new GameBoard(x | newBit, o) 
            : new GameBoard(x, o | newBit);
    }

    private bool IsWin(CellState player)
    {
        var board = player == CellState.X ? x : o;
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
        return ((x | o) & BoardMask) == BoardMask;
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
        ulong topRow = (x | o) & TopRowMask;  // Check which columns are full
        for (int col = 0; col < Columns; col++)
        {
            if ((topRow & (1UL << (col * BitsPerColumn + Rows - 1))) == 0)
                moves[count++] = col;
        }
    }

    public int HalfTurn => BitOperations.PopCount(x | o);

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
        if (((x | o) & BoardMask) == BoardMask)
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
        var result = new HashSet<(int row, int col)>();
        for (var row = 0; row < Rows; row++)
        {
            for (var col = 0; col < Columns; col++)
            {
                if (this[row, col] != player)
                {
                    continue;
                }
                // Horizontal
                if (col <= Columns - 4 && Enumerable.Range(0, 4).All(d => this[row, col + d] == player))
                {
                    for (var d = 0; d < 4; d++)
                    {
                        result.Add((row, col + d));
                    }

                    return result;
                }
                // Vertical
                if (row <= Rows - 4 && Enumerable.Range(0, 4).All(d => this[row + d, col] == player))
                {
                    for (var d = 0; d < 4; d++)
                    {
                        result.Add((row + d, col));
                    }

                    return result;
                }
                // Diagonal /
                if (row <= Rows - 4 && col >= 3 && Enumerable.Range(0, 4).All(d => this[row + d, col - d] == player))
                {
                    for (var d = 0; d < 4; d++)
                    {
                        result.Add((row + d, col - d));
                    }

                    return result;
                }
                // Diagonal \
                if (row <= Rows - 4 && col <= Columns - 4 && Enumerable.Range(0, 4).All(d => this[row + d, col + d] == player))
                {
                    for (var d = 0; d < 4; d++)
                    {
                        result.Add((row + d, col + d));
                    }

                    return result;
                }
            }
        }
        return result;
    }

   
}
