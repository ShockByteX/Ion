using Ion.Interop.Handles;
using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;

namespace Ion.Interop;

internal static unsafe partial class Ntdll
{
    public const uint FileAttributeNormal = 0x80;

    private const string LibraryName = "ntdll.dll";

    [LibraryImport(LibraryName), DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
    public static partial NtStatus NtClose(nint handle);

    [LibraryImport(LibraryName), DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
    public static partial NtStatus NtCreateFile(out nint FileHandle,
    FileAccessRights DesiredAccess,
    ObjectAttributes* ObjectAttributes,
    IoStatusBlock* IoStatusBlock,
    nint AllocationSize,
    uint FileAttributes,
    FileShare ShareAccess,
    FileCreateDisposition CreateDisposition,
    FileCreateOptions CreateOptions,
    nint EaBuffer,
    uint EaLength);

    [SuppressUnmanagedCodeSecurity]
    [SuppressGCTransition]
    [LibraryImport(LibraryName), DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
    public static partial NtStatus NtDeviceIoControlFile(nint FileHandle, 
        [Optional] nint Event, 
        [Optional] nint ApcRoutine, 
        [Optional] nint ApcContext, 
        IoStatusBlock* IoStatusBlock, 
        uint IoControlCode, 
        [Optional] nint InputBuffer, 
        uint InputBufferLength, 
        [Optional] nint OutputBuffer, 
        uint OutputBufferLength);
}