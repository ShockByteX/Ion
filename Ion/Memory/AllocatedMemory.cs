using Ion.Extensions;
using Ion.Native;
using Ion.Validation;

namespace Ion.Memory;

public interface IAllocatedMemory : IMemoryPointer, IDisposable
{
    int Size { get; }
}

internal sealed class AllocatedMemory : MemoryPointer, IAllocatedMemory
{
    private AllocatedMemory(IProcessMemory memory, IntPtr address, int size) : base(memory, address)
    {
        Size = size;
    }

    public int Size { get; }

    public void Dispose()
    {
        Memory.Process.Handle.ReleaseMemoryPage(Address, Size);
    }

    public static IAllocatedMemory Allocate(IProcessMemory memory, int size, MemoryAllocationFlags allocation, MemoryProtectionFlags protection)
    {
        var address = Kernel32.VirtualAllocEx(memory.Process.Handle, IntPtr.Zero, size, allocation, protection);

        Assert.Win32(address != IntPtr.Zero);

        return new AllocatedMemory(memory, address, size);
    }
} 