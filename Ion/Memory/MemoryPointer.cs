using System.Text;

namespace Ion.Memory;

public interface IMemoryPointer : IEquatable<IMemoryPointer>
{
    IntPtr Address { get; }
    IProcessMemory Memory { get; }

    bool IsValid { get; }

    byte[] Read(int offset, int length);
    T Read<T>(int offset);
    T[] Read<T>(int offset, int length);
    string Read(int offset, Encoding encoding, int maxLength);
    string Read(int offset, Encoding encoding);

    void Write(int offset, string text, Encoding encoding);
    void Write<T>(int offset, T value);
    void Write<T>(int offset, T[] values);
    int Write(int offset, byte[] data);
}

internal class MemoryPointer : IEquatable<MemoryPointer>, IMemoryPointer
{
    public MemoryPointer(IProcessMemory memory, IntPtr address)
    {
        Memory = memory;
        Address = address;
    }

    public IProcessMemory Memory { get; }
    public IntPtr Address { get; protected set; }
    public virtual bool IsValid => Address != IntPtr.Zero;

    public byte[] Read(int offset, int length) => Memory.Read(Address + offset, length);
    public T Read<T>(int offset) => Memory.Read<T>(Address + offset);
    public T[] Read<T>(int offset, int count) => Memory.Read<T>(Address + offset, count);
    public string Read(int offset, Encoding encoding, int maxLength) => Memory.Read(Address + offset, encoding, maxLength);
    public string Read(int offset, Encoding encoding) => Memory.Read(Address + offset, encoding);

    public int Write(int offset, byte[] data) => Memory.Write(Address + offset, data);
    public void Write<T>(int offset, T value) => Memory.Write(Address + offset, value);
    public void Write<T>(int offset, T[] values) => Memory.Write(Address + offset, values);
    public void Write(int offset, string text, Encoding encoding) => Memory.Write(Address + offset, text, encoding);

    public bool Equals(MemoryPointer? other) => Equals((IMemoryPointer?)other);
    public bool Equals(IMemoryPointer? other) => other is not null && Address == other.Address;
    public override bool Equals(object? obj) => obj is IMemoryPointer pointer && Equals(pointer);
    public override int GetHashCode() => Address.GetHashCode();

    public override string ToString() => $"Address: 0x{Address.ToInt64():X}";

    public static bool operator ==(MemoryPointer left, MemoryPointer right) => Equals(left, right);
    public static bool operator !=(MemoryPointer left, MemoryPointer right) => !Equals(left, right);
}