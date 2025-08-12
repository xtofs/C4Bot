# Development Notes

## Smart Enum Pattern Implementation

### Overview

This project implements a "smart enum" pattern using structs instead of traditional C# enums for `CellState`, `Player`, `GameState`, and `GameResult`. This provides enhanced type safety and cross-type comparisons while maintaining performance.

### Pattern Structure

Each smart enum follows this structure:

```csharp
public readonly struct SmartEnum : IEquatable<SmartEnum>
{
    // Public inner enum for switch statements (compile-time constants)
    public enum Values : byte { Value1, Value2, Value3 }
    
    // Static readonly instances for easy access
    public static readonly SmartEnum Value1 = new(Values.Value1);
    public static readonly SmartEnum Value2 = new(Values.Value2);
    
    // Internal value storage
    public Values Value { get; }
    
    // Constructor
    private SmartEnum(Values value) => Value = value;
    
    // Implicit conversions
    public static implicit operator SmartEnum(Values value) => new(value);
    public static implicit operator Values(SmartEnum smartEnum) => smartEnum.Value;
    
    // Equality operators
    public static bool operator ==(SmartEnum left, SmartEnum right) => left.Value == right.Value;
    public static bool operator !=(SmartEnum left, SmartEnum right) => left.Value != right.Value;
    
    // IEquatable implementation
    public bool Equals(SmartEnum other) => Value == other.Value;
    public override bool Equals(object? obj) => obj is SmartEnum other && Equals(other);
    public override int GetHashCode() => Value.GetHashCode();
}
```

### Benefits

1. **Type Safety**: Prevents accidental comparisons between incompatible types
2. **Cross-type Equality**: Enables natural comparisons like `Player.X == CellState.X`
3. **Implicit Conversions**: Works seamlessly where underlying enums were used
4. **Better IntelliSense**: Static readonly instances provide better code completion
5. **Performance**: Struct-based implementation with no heap allocations
6. **Debugger Support**: Can include custom `DebuggerDisplay` attributes

### Trade-offs and Limitations

#### Switch Statement Limitation

**Problem**: Static readonly properties cannot be used as case labels in switch statements because they're not compile-time constants.

```csharp
// ❌ This doesn't work - not compile-time constants
switch (cellState)
{
    case CellState.X: // Error! Not a constant
    case CellState.O: // Error! Not a constant
}
```

**Solution**: Use the public inner `Values` enum for switch statements:

```csharp
// ✅ This works - using the public inner enum
switch (cellState.Value)
{
    case CellState.Values.X: // OK - enum constant
    case CellState.Values.O: // OK - enum constant
}
```

#### Why the Inner Enum Must Be Public

The inner `Values` enum is public because:

1. **Switch statements need compile-time constants** - only enum values qualify
2. **Pattern matching requires accessible constants** - for use in switch expressions
3. **Clean access to underlying values** - provides `obj.Value` for when needed
4. **Backward compatibility** - allows migration from simple enums

### Usage Patterns

#### For Equality Comparisons (Recommended)

```csharp
if (cellState == CellState.X)     // ✅ Clean and type-safe
if (cellState == Player.X)        // ✅ Cross-type equality works
```

#### For Switch Statements (Required)

```csharp
switch (cellState.Value)
{
    case CellState.Values.X:       // ✅ Uses compile-time constants
    case CellState.Values.O:
    case CellState.Values.Empty:
}
```

#### For Method Parameters

```csharp
// Both work due to implicit conversions
board.ApplyMove(col, Player.X);           // ✅ Smart struct
board.ApplyMove(col, CellState.X);        // ✅ Implicit conversion
```

### Implementation Examples

#### CellState and Player

- `Player`: X, O (represents active players, no Empty state)
- `CellState`: Empty, X, O (represents board cell state)
- Cross-type equality: `Player.X == CellState.X` returns `true`

#### GameState and GameResult  

- `GameResult`: XWin, OWin, Draw (terminal states only, no Ongoing)
- `GameState`: Ongoing, XWin, OWin, Draw (all possible game states)
- Cross-type equality: `GameState.XWin == GameResult.XWin` returns `true`

### Migration Strategy

When converting from simple enums to smart structs:

1. **Replace switch expressions**: Change `GameResult.WinX` to `result.Value` and use `GameResult.Values.XWin`
2. **Update method calls**: Most work automatically due to implicit conversions
3. **Fix equality operators**: Ensure they compare `.Value` directly to avoid recursion
4. **Test thoroughly**: Verify switch statements, equality comparisons, and method calls

### Performance Considerations

- **Struct-based**: No heap allocations, passed by value
- **Inline operations**: Simple equality and conversion operations
- **Cache-friendly**: Small memory footprint (single byte for most enums)
- **Switch optimization**: Compiler can optimize switch on enum values efficiently

This pattern provides a sophisticated and type-safe approach while maintaining the practical needs of switch statement usage and performance requirements.
