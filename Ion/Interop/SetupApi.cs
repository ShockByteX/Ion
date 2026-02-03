using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Ion.Interop;

internal static unsafe partial class SetupApi
{
    private const string LibraryName = "setupapi.dll";

    [LibraryImport(LibraryName, SetLastError = true, StringMarshalling = StringMarshalling.Utf16)]
    public static partial nint SetupDiGetClassDevsW(Guid* ClassGuid, string? Enumerator, nint hwndParent, SetupDiGetClassDevsFlags Flags);

    [LibraryImport(LibraryName, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool SetupDiEnumDeviceInfo(nint DeviceInfoSet, uint MemberIndex, ref SP_DEVINFO_DATA DeviceInfoData);

    [LibraryImport(LibraryName, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool SetupDiGetDeviceRegistryPropertyW(nint DeviceInfoSet,
        ref SP_DEVINFO_DATA DeviceInfoData,
        SetupDiRegistryProperty Property,
        out uint PropertyRegDataType,
        byte[]? PropertyBuffer,
        uint PropertyBufferSize,
        out uint RequiredSize);

    [LibraryImport(LibraryName, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool SetupDiDestroyDeviceInfoList(nint DeviceInfoSet);
}