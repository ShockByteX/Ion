using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Ion.Marshaling;
using Ion.Native;
using Microsoft.Win32.SafeHandles;

namespace Ion.ConsoleTest;

internal sealed unsafe class ProcessMemory : IDisposable
{
    private const string Kernel32LibraryName = "kernel32.dll";

    [DllImport(Kernel32LibraryName, ExactSpelling = true, SetLastError = true)]
    private static extern SafeProcessHandle OpenProcess(ProcessAccessFlags dwDesiredAccess, [MarshalAs(UnmanagedType.Bool)] bool bInheritHandle, int dwProcessId);

    [DllImport(Kernel32LibraryName, SetLastError = true)]
    private static extern bool ReadProcessMemory(SafeProcessHandle hProcess, nint lpBaseAddress, void* lpBuffer, int nSize, out nint lpNumberOfBytesRead);

    private readonly SafeProcessHandle _handle;

    public ProcessMemory(int processId)
    {
        _handle = OpenProcess(ProcessAccessFlags.AllAccess, bInheritHandle: false, processId);

        Ensure.That(!_handle.IsInvalid, () => $"Failed to open process: {processId}");
    }

    public T Read<T>(nint address) where T : struct
    {
        Ensure.That(address != nint.Zero, () => $"Failed to read memory at: {address:x}");

        var size = Unsafe.SizeOf<T>();
        Span<byte> buffer = stackalloc byte[size];

        fixed (byte* pBuffer = buffer)
        {
            Ensure.Win32(ReadProcessMemory(_handle, address, pBuffer, size, out var bytesRead) && bytesRead == size, () => $"Failed to read memory at: {address:x} (Size: {size})");
        }

        return MemoryMarshal.Read<T>(buffer);
    }

    public bool TryRead<T>(nint address, out T value) where T : struct
    {
        try
        {
            value = Read<T>(address);
            return true;
        }
        catch (Exception e) when (!e.IsCritical())
        {
            value = default;
            return false;
        }
    }

    public void Dispose()
    {
        _handle.Dispose();
    }
}