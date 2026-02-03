using Ion.Marshaling;
using Ion.Interop;
using Ion.Validation;

namespace Ion.Memory;

public sealed class MemoryObject<T> : IDisposable where T : struct
{
    private readonly IProcess _process;

    public MemoryObject(IProcess process, nint address, int size)
    {
        _process = process;
        Address = address;
        Size = size;
    }

    public nint Address { get; }
    public int Size { get; }

    public T GetValue<T>() where T : struct
    {
        Ensure.That(MarshalType<T>.Size == Size);
        return _process[Address].Read<T>(0);
    }

    public void SetValue<T>(T value) where T : struct
    {
        Ensure.That(MarshalType<T>.Size == Size);
        _process[Address].Write(0, value);
    }

    public void Dispose()
    {
        Kernel32.VirtualFreeEx(_process.Handle, Address, Size, MemoryReleaseFlags.Release);
    }

    internal static MemoryObject<T> Allocate(IProcess process, int size)
    {
        var address = Kernel32.VirtualAllocEx(process.Handle, nint.Zero, size, MemoryAllocationFlags.Commit, MemoryProtectionFlags.ReadWrite);
        return new MemoryObject<T>(process, address, size);
    }
}