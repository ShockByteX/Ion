using Ion.Interop;
using Ion.Interop.Primitives;
using Ion.Validation;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Ion.Devices;

public static partial class DosDevices
{
    private const int DefaultBufferSize = 512;

    public static IReadOnlyCollection<string> List(string? deviceName = null)
    {
        var error = Win32Error.InsufficientBuffer;
        var buffer = new char[DefaultBufferSize];

        var length = 0;

        while (error is Win32Error.InsufficientBuffer)
        {
            length = Kernel32.QueryDosDeviceW(deviceName, buffer, (uint)buffer.Length);
            error = length > 0 ? Win32Error.Success : (Win32Error)Marshal.GetLastWin32Error();

            if (error is Win32Error.InsufficientBuffer)
                Array.Resize(ref buffer, buffer.Length << 1);
        }

        Ensure.Win32(error);

        return SplitMultiSz(buffer.AsSpan()[..length]);
    }

    private static IReadOnlyCollection<string> SplitMultiSz(ReadOnlySpan<char> buffer)
    {
        var splitted = buffer.Split('\0');
        var result = new List<string>();

        foreach (var range in splitted)
        {
            var part = buffer[range];

            if (!part.IsEmpty)
                result.Add(part.ToString());
        }

        return result;
    }
}