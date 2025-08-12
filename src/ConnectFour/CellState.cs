namespace ConnectFour;

// Smart structs `Player = X|O` and `CellState = X|O|Empty` with
//      private constructors
//      Inner `Values` enum
//      Static readonly instances
//      Implicit conversion operators
//      Proper IEquatable implementations
//      Equality comparisons (including cross-type with related struct)



/// <summary>
/// Represents a Connect Four player. The valid values are X and O.
/// we have a related CellState struct that represents the state of a cell on the board (X, O, or Empty)
/// Values of Player can be compared and converted to CellState (with limitations).
/// </summary>
public readonly struct Player : IEquatable<Player>, IEquatable<CellState>
{

    public enum Values : byte { X = 1, O = 2 }

    public Values Value { get; }

    private Player(Values value) => Value = value;

    public static readonly Player X = new Player(Values.X);
    public static readonly Player O = new Player(Values.O);

    public override string ToString() => Value switch
    {
        Values.X => "X",
        Values.O => "O",
        _ => throw new ArgumentOutOfRangeException(nameof(Value), "Invalid CellState value")
    };

    public bool Equals(Player other)
    {
        return Value == other.Value;
    }

    public bool Equals(CellState other)
    {
        // this works only because of the carefully crafted enums Player: 1, 2 and CellState: 0, 1, 2
        return (byte)Value == (byte)other.Value;
    }

    /// <summary>
    /// Implicitly converts a CellState to a Player (returns null for Empty).
    /// </summary>
    public static implicit operator Player?(CellState state) => state.Value switch
    {
        CellState.Values.Empty => null,
        CellState.Values.X => Player.X,
        CellState.Values.O => Player.O,
        _ => throw new ArgumentOutOfRangeException(nameof(state), state, "Invalid CellState value")
    };

    public override bool Equals(object? obj)
    {
        return (obj is Player player && Equals(player)) || (obj is CellState cellState && Equals(cellState));
    }

    public static bool operator ==(Player left, Player right)
    {
        return left.Value == right.Value;
    }

    public static bool operator !=(Player left, Player right)
    {
        return !(left == right);
    }

    public override int GetHashCode() => Value.GetHashCode();

    internal Player Opponent()
    {
        return this.Value switch
        {
            Player.Values.X => Player.O,
            Player.Values.O => Player.X,
            _ => throw new InvalidDataException("internal state of Player is corrupted")
        };
    }
}

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
