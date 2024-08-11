using System.ComponentModel;
using Ion.Handles;
using Ion.Memory;
using Ion.Native;
using Ion.Properties;
using Ion.Validation;

namespace Ion.Extensions;

internal static class SafeProcessHandleExtension
{
    public static byte[] ReadMemory(this SafeProcessHandle handle, IntPtr address, int length)
    {
        Assert.IsValid(handle);
        Assert.IsValid(address);

        var data = new byte[length];
        var errorMessage = string.Format(Resources.ErrorFailedToReadFrom, address.ToHexadecimalString());

        Assert.Win32(Kernel32.ReadProcessMemory(handle, address, data, length, out var nbBytesRead) && length == nbBytesRead, errorMessage);

        return data;
    }

    public static int WriteMemory(this SafeProcessHandle handle, IntPtr address, byte[] data)
    {
        return handle.WriteMemoryProtected(address, data.Length, () =>
        {
            Assert.Win32(Kernel32.WriteProcessMemory(handle, address, data, data.Length, out var written));
            Assert.Win32(written == data.Length);
        });
    }

    public static int WriteMemoryProtected(this SafeProcessHandle handle, IntPtr address, int length, Action write)
    {
        Assert.IsValid(handle);
        Assert.IsValid(address);

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

    public static MemoryProtectionFlags GetMemoryProtection(this SafeProcessHandle handle, IntPtr address)
    {
        Assert.Win32(Query(handle, address, out var memoryInfo) > 0);

        return memoryInfo.MemoryProtection;
    }

    public static int Query(this SafeProcessHandle handle, IntPtr address, out MemoryBasicInformation memoryInfo)
    {
        Assert.IsValid(handle);

        var result = Kernel32.VirtualQueryEx(handle, address, out memoryInfo);

        return result;
    }

    public static void ReleaseMemoryPage(this SafeProcessHandle handle, IntPtr address)
    {
        Assert.IsValid(handle);
        Assert.IsValid(address);
        Assert.Win32(Kernel32.VirtualFreeEx(handle, address, 0, MemoryReleaseFlags.Release), string.Format(Resources.ErrorFailedToReleaseMemoryPage, address.ToHexadecimalString()));
    }

    public static void ReleaseMemoryPage(this SafeProcessHandle handle, IntPtr address, int size)
    {
        Assert.IsValid(handle);
        Assert.IsValid(address);
        Assert.Win32(Kernel32.VirtualFreeEx(handle, address, size, MemoryReleaseFlags.Decommit), string.Format(Resources.ErrorFailedToReleaseMemoryPage, address.ToHexadecimalString()));
    }
}