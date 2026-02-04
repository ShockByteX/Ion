using Ion.Interop;
using Ion.Interop.Handles;
using Ion.Validation;

namespace Ion.Memory;

public interface IMemoryProtection : IDisposable
{
    nint Address { get; }
    int Size { get; }
    PageProtectionFlags Protection { get; }
    PageProtectionFlags OldProtection { get; }
}

internal sealed class MemoryProtection : IMemoryProtection
{
    private readonly SafeProcessHandle _handle;

    private bool _disposed;

    private MemoryProtection(SafeProcessHandle handle, nint address, int size, PageProtectionFlags protection, PageProtectionFlags oldProtection)
    {
        _handle = handle;
        Address = address;
        Size = size;
        Protection = protection;
        OldProtection = oldProtection;
    }

    public nint Address { get; }
    public int Size { get; }
    public PageProtectionFlags Protection { get; }
    public PageProtectionFlags OldProtection { get; }

    public void Dispose()
    {
        if (!_disposed)
        {
            if (!_handle.IsClosed && !_handle.IsInvalid)
                Ensure.Win32(Kernel32.VirtualProtectEx(_handle, Address, (nuint)Size, OldProtection, out _));

            _disposed = true;
        }
    }

    public override string ToString() => $"BaseAddress: 0x{Address.ToInt64():X}, Protection: {Protection}, OldProtection: {OldProtection}";

    public static IMemoryProtection Change(SafeProcessHandle handle, nint address, int size, PageProtectionFlags protection = PageProtectionFlags.ExecuteReadWrite)
    {
        Ensure.IsValid(handle);
        Ensure.IsValid(address);
        Ensure.Win32(Kernel32.VirtualProtectEx(handle, address, (nuint)size, protection, out var oldProtection));

        return new MemoryProtection(handle, address, size, protection, oldProtection);
    }
}