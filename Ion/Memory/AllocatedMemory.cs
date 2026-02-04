using Ion.Extensions;
using Ion.Interop;
using Ion.Validation;

namespace Ion.Memory;

public interface IAllocatedMemory : IMemoryPointer, IDisposable
{
    int Size { get; }
}

internal sealed class AllocatedMemory : MemoryPointer, IAllocatedMemory
{
    private AllocatedMemory(IProcessMemory memory, nint address, int size) : base(memory, address)
    {
        Size = size;
    }

    public int Size { get; }

    public void Dispose()
    {
        Memory.Process.Handle.ReleaseMemoryPage(Address, Size);
    }

    public static IAllocatedMemory Allocate(IProcessMemory memory, int size, MemoryAllocationFlags allocation, PageProtectionFlags protection)
    {
        var address = Kernel32.VirtualAllocEx(memory.Process.Handle, nint.Zero, size, allocation, protection);

        Ensure.Win32(address != nint.Zero);

        return new AllocatedMemory(memory, address, size);
    }
} 