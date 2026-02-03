namespace Ion.Memory;

public interface IMemoryValue<T> : IEquatable<IMemoryValue<T>>
{
    nint Address { get; }
    T Value { get; set; }
}

internal sealed class MemoryValue<T> : IEquatable<MemoryValue<T>>, IMemoryValue<T> where T : struct
{
    private readonly IProcessMemory _memory;

    public MemoryValue(IProcessMemory memory, nint address)
    {
        _memory = memory;
        Address = address;
    }

    public nint Address { get; }
    public T Value { get => _memory.Read<T>(Address); set => _memory.Write(Address, value); }

    public bool Equals(MemoryValue<T>? other) => Equals((IMemoryValue<T>?)other);
    public bool Equals(IMemoryValue<T>? other) => other is not null && Address == other.Address;
    public override bool Equals(object? obj) => obj is IMemoryValue<T> memoryValue && Equals(memoryValue);
    public override int GetHashCode() => Address.GetHashCode();
    public override string ToString() => $"Address: 0x{Address.ToInt64():X}, Value: {Value}";
}