using System.ComponentModel;
using Ion.Handles;
using Ion.Memory;
using Ion.Native;
using Ion.Properties;
using Ion.Validation;

namespace Ion.Extensions;

internal static class SafeProcessHandleExtension
{
    public static byte[] ReadMemory(this SafeProcessHandle handle, nint address, int length)
    {
        Ensure.IsValid(handle);
        Ensure.IsValid(address);

        var data = new byte[length];
        var errorMessage = string.Format(Resources.ErrorFailedToReadFrom, address.ToHexadecimalString());

        Ensure.Win32(Kernel32.ReadProcessMemory(handle, address, data, length, out var nbBytesRead) && length == nbBytesRead, errorMessage);

        return data;
    }

    public static int WriteMemory(this SafeProcessHandle handle, nint address, byte[] data)
    {
        return handle.WriteMemoryProtected(address, data.Length, () =>
        {
            Ensure.Win32(Kernel32.WriteProcessMemory(handle, address, data, data.Length, out var written));
            Ensure.Win32(written == data.Length);
        });
    }

    public static int WriteMemoryProtected(this SafeProcessHandle handle, nint address, int length, Action write)
    {
        Ensure.IsValid(handle);
        Ensure.IsValid(address);

        IMemoryProtection? protection = null;

        try
        {
            protection = MemoryProtection.Change(handle, address, length);

            write();

            return length;
        }
        catch (Exception e)
        {
            throw new Win32Exception(string.Format(Resources.ErrorFailedToWriteTo, address.ToHexadecimalString()), e);
        }
        finally
        {
            protection?.Dispose();
        }
    }

    public static MemoryProtectionFlags GetMemoryProtection(this SafeProcessHandle handle, nint address)
    {
        Ensure.Win32(Query(handle, address, out var memoryInfo) > 0);

        return memoryInfo.MemoryProtection;
    }

    public static int Query(this SafeProcessHandle handle, nint address, out MemoryBasicInformation memoryInfo)
    {
        Ensure.IsValid(handle);

        var result = Kernel32.VirtualQueryEx(handle, address, out memoryInfo);

        return result;
    }

    public static void ReleaseMemoryPage(this SafeProcessHandle handle, nint address)
    {
        Ensure.IsValid(handle);
        Ensure.IsValid(address);
        Ensure.Win32(Kernel32.VirtualFreeEx(handle, address, 0, MemoryReleaseFlags.Release), string.Format(Resources.ErrorFailedToReleaseMemoryPage, address.ToHexadecimalString()));
    }

    public static void ReleaseMemoryPage(this SafeProcessHandle handle, nint address, int size)
    {
        Ensure.IsValid(handle);
        Ensure.IsValid(address);
        Ensure.Win32(Kernel32.VirtualFreeEx(handle, address, size, MemoryReleaseFlags.Decommit), string.Format(Resources.ErrorFailedToReleaseMemoryPage, address.ToHexadecimalString()));
    }
}