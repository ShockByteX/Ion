using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
using System.Text;

namespace Ion.Interop;

internal static unsafe partial class Ntdll
{
    public const uint FileAttributeNormal = 0x80;

    private const string LibraryName = "ntdll.dll";

    [LibraryImport(LibraryName)]
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

    [LibraryImport(LibraryName)]
    public static partial NtStatus NtClose(nint handle);
}