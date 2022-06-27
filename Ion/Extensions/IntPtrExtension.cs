using System.ComponentModel;
using System.Runtime.CompilerServices;
using Ion.Handles;
using Ion.Marshaling;
using Ion.Native;
using Ion.Properties;
using Ion.Validation;

namespace Ion.Extensions;

internal static class IntPtrExtension
{
    public static IntPtr Add(this IntPtr address, long value) => new(address.ToInt64() + value);

    public static string ToHexadecimalString(this IntPtr address) => $"0x{address.ToInt64():X}";

    public static unsafe byte[] ReadMemory(this IntPtr address, int length, bool copy = false)
    {
        Assert.IsValid(address);

        try
        {
            if (copy)
            {

                var data = new byte[length];

                fixed (byte* pData = data)
                {
                    Msvcrt.memcpy((IntPtr)pData, address, length);
                }

                return data;
            }

            return Unsafe.Read<byte[]>(address.ToPointer());
        }
        catch (Exception e)
        {
            throw new Win32Exception(string.Format(Resources.ErrorFailedToReadFrom, address.ToHexadecimalString(), e));
        }
    }

    public static unsafe int WriteMemory(this IntPtr address, SafeProcessHandle handle, byte[] data, bool copy = false)
    {
        return handle.WriteMemoryProtected(address, data.Length, () =>
        {
            if (copy)
            {
                fixed (byte* pData = data)
                {
                    Msvcrt.memcpy(address, (IntPtr)pData, data.Length);
                }
            }
            else
            {
                Unsafe.Write(address.ToPointer(), data);
            }
        });
    }

    public static unsafe T Read<T>(this IntPtr address)
    {
        try
        {
            Assert.IsValid(address);
            return Unsafe.Read<T>(address.ToPointer());
        }
        catch (AccessViolationException e)
        {
            throw new Win32Exception(string.Format(Resources.ErrorFailedToReadFrom, address.ToHexadecimalString()), e);
        }
    }

    public static unsafe int Write<T>(this IntPtr address, SafeProcessHandle handle, T value)
    {
        return handle.WriteMemoryProtected(address, MarshalType<T>.Size, () =>
        {
            Unsafe.Write(address.ToPointer(), value);
        });
    }
}