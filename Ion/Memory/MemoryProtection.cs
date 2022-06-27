﻿using Ion.Handles;
using Ion.Native;
using Ion.Validation;

namespace Ion.Memory;

public interface IMemoryProtection : IDisposable
{
    IntPtr Address { get; }
    int Size { get; }
    MemoryProtectionFlags Protection { get; }
    MemoryProtectionFlags OldProtection { get; }
}

internal sealed class MemoryProtection : IMemoryProtection
{
    private readonly SafeProcessHandle _handle;

    private bool _disposed;

    private MemoryProtection(SafeProcessHandle handle, IntPtr address, int size, MemoryProtectionFlags protection, MemoryProtectionFlags oldProtection)
    {
        _handle = handle;
        Address = address;
        Size = size;
        Protection = protection;
        OldProtection = oldProtection;
    }

    ~MemoryProtection() => Dispose();

    public IntPtr Address { get; }
    public int Size { get; }
    public MemoryProtectionFlags Protection { get; }
    public MemoryProtectionFlags OldProtection { get; }

    public void Dispose()
    {
        if (!_disposed)
        {
            if (!_handle.IsClosed && !_handle.IsInvalid)
            {
                Assert.Win32(Kernel32.VirtualProtectEx(_handle, Address, Size, OldProtection, out _));
            }

            _disposed = true;
            GC.SuppressFinalize(this);
        }
    }

    public override string ToString() => $"BaseAddress: 0x{Address.ToInt64():X}, Protection: {Protection}, OldProtection: {OldProtection}";

    public static IMemoryProtection Change(SafeProcessHandle handle, IntPtr address, int size, MemoryProtectionFlags protection = MemoryProtectionFlags.ExecuteReadWrite)
    {
        Assert.IsValid(handle);
        Assert.IsValid(address);
        Assert.OutOfRange(size, 0, int.MaxValue, nameof(size));
        Assert.Win32(Kernel32.VirtualProtectEx(handle, address, size, MemoryProtectionFlags.ExecuteReadWrite, out var oldProtection));

        return new MemoryProtection(handle, address, size, protection, oldProtection);
    }
}