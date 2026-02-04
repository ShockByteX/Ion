using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Text;

namespace Ion.Memory;

public interface IMemoryPointer : IEquatable<IMemoryPointer>
{
    nint Address { get; }
    IProcessMemory Memory { get; }

    bool IsValid { get; }

    byte[] Read(int offset, int length);
    T Read<[DynamicallyAccessedMembers(DynamicallyAccessedMembers.Default)] T>(int offset) where T : unmanaged;
    T[] Read<[DynamicallyAccessedMembers(DynamicallyAccessedMembers.Default)] T>(int offset, int length) where T : unmanaged;
    string Read(int offset, Encoding encoding, int maxLength);
    string Read(int offset, Encoding encoding);

    void Write(int offset, string text, Encoding encoding);
    void Write<[DynamicallyAccessedMembers(DynamicallyAccessedMembers.Default)] T>(int offset, in T value) where T : unmanaged;
    void Write<[DynamicallyAccessedMembers(DynamicallyAccessedMembers.Default)] T>(int offset, T[] values) where T : unmanaged;
    int Write(int offset, ReadOnlySpan<byte> data);
}

internal class MemoryPointer(IProcessMemory memory, nint address) : IEquatable<MemoryPointer>, IMemoryPointer
{
    public IProcessMemory Memory { get; } = memory;
    public nint Address { get; protected set; } = address;
    public virtual bool IsValid => Address != nint.Zero;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public byte[] Read(int offset, int length) => Memory.Read(Address + offset, length);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T Read<[DynamicallyAccessedMembers(DynamicallyAccessedMembers.Default)] T>(int offset) where T : unmanaged => Memory.Read<T>(Address + offset);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T[] Read<[DynamicallyAccessedMembers(DynamicallyAccessedMembers.Default)] T>(int offset, int count) where T : unmanaged => Memory.Read<T>(Address + offset, count);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public string Read(int offset, Encoding encoding, int maxLength) => Memory.Read(Address + offset, encoding, maxLength);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public string Read(int offset, Encoding encoding) => Memory.Read(Address + offset, encoding);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int Write(int offset, ReadOnlySpan<byte> data) => Memory.Write(Address + offset, data);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Write<[DynamicallyAccessedMembers(DynamicallyAccessedMembers.Default)] T>(int offset, in T value) where T : unmanaged => Memory.Write(Address + offset, value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Write<[DynamicallyAccessedMembers(DynamicallyAccessedMembers.Default)] T>(int offset, T[] values) where T : unmanaged => Memory.Write(Address + offset, values);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Write(int offset, string text, Encoding encoding) => Memory.Write(Address + offset, text, encoding);

    public bool Equals(MemoryPointer? other) => Equals((IMemoryPointer?)other);
    public bool Equals(IMemoryPointer? other) => other is not null && Address == other.Address;
    public override bool Equals(object? obj) => obj is IMemoryPointer pointer && Equals(pointer);
    public override int GetHashCode() => Address.GetHashCode();

    public override string ToString() => $"Address: 0x{Address.ToInt64():X}";

    public static bool operator ==(MemoryPointer left, MemoryPointer right) => Equals(left, right);
    public static bool operator !=(MemoryPointer left, MemoryPointer right) => !Equals(left, right);
}