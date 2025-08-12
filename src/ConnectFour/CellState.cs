namespace ConnectFour;

/// <summary>
/// Represents the state of a cell on the Connect Four board. One of X, O, or Empty.
/// Values of CellState can be compared and converted to Players (with limitations).
/// </summary>
public readonly struct CellState
{
    public Values Value { get; }

    public enum  Values :byte { Empty = 0, X = 1, O = 2 }

    private CellState(Values value) => Value = value;

    public static readonly CellState Empty = new CellState(Values.Empty);
    public static readonly CellState X = new CellState(Values.X);
    public static readonly CellState O = new CellState(Values.O);

    public override string ToString() => Value switch
    {
        Values.Empty => "Empty",
        Values.X => "X",
        Values.O => "O",
        _ => throw new ArgumentOutOfRangeException(nameof(Value), "Invalid CellState value")
    };

    public static implicit operator CellState(Player player) => player.Value switch
    {
        Player.Values.X => X,
        Player.Values.O => O,
        _ => throw new ArgumentOutOfRangeException(nameof(player), player, "Invalid Player value")
    };

    public override bool Equals(object? obj)
    {
        return obj is Player player && Equals(player) || obj is CellState cellState && Equals(cellState);
    }

    public static bool operator ==(CellState left, CellState right)
    {
        return left.Value == right.Value;
    }

    public static bool operator !=(CellState left, CellState right)
    {
        return !(left == right);
    }

    public override int GetHashCode() => Value.GetHashCode();
}
