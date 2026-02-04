using System.ComponentModel;
using System.Runtime.CompilerServices;
using Ion.Marshaling;
using Ion.Interop;
using Ion.Properties;
using Ion.Validation;
using Ion.Interop.Handles;

namespace Ion.Extensions;

internal static class IntPtrExtension
{
    public static nint Add(this nint address, long value) => new(address.ToInt64() + value);

    public static string ToHexadecimalString(this nint address) => $"0x{address.ToInt64():X}";

    public static unsafe byte[] ReadMemory(this nint address, int length, bool copy = false)
    {
        Ensure.IsValid(address);

        try
        {
            if (copy)
            {

                var data = new byte[length];

                fixed (byte* pData = data)
                {
                    Msvcrt.memcpy((nint)pData, address, length);
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

    public static unsafe int WriteMemory(this nint address, SafeProcessHandle handle, ReadOnlySpan<byte> data, bool copy = false)
    {
        throw new NotImplementedException();
        //return handle.WriteMemoryProtected(address, data.Length, () =>
        //{
        //    if (copy)
        //    {
        //        fixed (byte* pData = data)
        //        {
        //            Msvcrt.memcpy(address, (nint)pData, data.Length);
        //        }
        //    }
        //    else
        //    {
        //        Unsafe.Write(address.ToPointer(), data);
        //    }
        //});
    }

    public static unsafe T Read<T>(this nint address)
    {
        try
        {
            Ensure.IsValid(address);
            return Unsafe.Read<T>(address.ToPointer());
        }
        catch (AccessViolationException e)
        {
            throw new Win32Exception(string.Format(Resources.ErrorFailedToReadFrom, address.ToHexadecimalString()), e);
        }
    }

    public static unsafe int Write<T>(this nint address, SafeProcessHandle handle, T value) where T : unmanaged
    {
        return handle.WriteMemoryProtected(address, sizeof(T), () =>
        {
            Unsafe.Write(address.ToPointer(), value);
        });
    }
}