using Ion.Interop;
using Ion.Interop.Primitives;
using Ion.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace Ion.Devices;

public static class DeviceManager
{
    private static readonly HashSet<Win32Error> AcceptableErrorCodes = [Win32Error.InvalidData, Win32Error.NotFound];

    public static unsafe bool TryFind(string name, [NotNullWhen(true)] out DeviceInfo? deviceInfo)
    {
        var set = SetupApi.SetupDiGetClassDevsW(null, null, 0, SetupDiGetClassDevsFlags.Present | SetupDiGetClassDevsFlags.AllClasses);

        if (set == nint.Zero || set == new nint(-1))
            throw new Win32Exception(Marshal.GetLastWin32Error());

        try
        {
            var info = new SP_DEVINFO_DATA { cbSize = (uint)Marshal.SizeOf<SP_DEVINFO_DATA>() };

            for (uint i = 0; SetupApi.SetupDiEnumDeviceInfo(set, i, ref info); i++)
            {
                var deviceName = ReadProperty(set, ref info, SetupDiRegistryProperty.FriendlyName)
                    ?? ReadProperty(set, ref info, SetupDiRegistryProperty.DeviceDescription);

                if (!string.Equals(deviceName, name, StringComparison.OrdinalIgnoreCase))
                    continue;

                deviceInfo = new DeviceInfo(Ensure.NotNull(deviceName),
                    Ensure.NotNull(ReadProperty(set, ref info, SetupDiRegistryProperty.Service)),
                    Ensure.NotNull(ReadProperty(set, ref info, SetupDiRegistryProperty.Driver)),
                    Ensure.NotNull(ReadProperty(set, ref info, SetupDiRegistryProperty.PhysicalDeviceObjectName)),
                    Ensure.NotNull(ReadProperty(set, ref info, SetupDiRegistryProperty.HardwareId)));

                return true;
            }

            deviceInfo = null;
            return false;
        }
        finally
        {
            SetupApi.SetupDiDestroyDeviceInfoList(set);
        }
    }

    private static string? ReadProperty(nint set, ref SP_DEVINFO_DATA data, SetupDiRegistryProperty property)
    {
        Ensure.That(!SetupApi.SetupDiGetDeviceRegistryPropertyW(set, ref data, property, out var type, null, 0, out var bufferSize));

        var errorCode = (Win32Error)Marshal.GetLastWin32Error();

        if (AcceptableErrorCodes.Contains(errorCode))
            return null;

        if (errorCode is not Win32Error.InsufficientBuffer)
            throw new Win32Exception((int)errorCode);

        var buffer = new byte[bufferSize];

        Ensure.Win32(SetupApi.SetupDiGetDeviceRegistryPropertyW(set, ref data, property, out type, buffer, bufferSize, out _));

        var text = Encoding.Unicode.GetString(buffer);
        var ntIndex = text.IndexOf('\0');

        return ntIndex >= 0 ? text[..ntIndex] : text;
    }
}