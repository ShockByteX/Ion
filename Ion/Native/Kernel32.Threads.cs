using Ion.Handles;
using System.Runtime.InteropServices;

using SafeHandle = Ion.Handles.SafeHandle;

namespace Ion.Native;

internal partial class Kernel32
{
    [DllImport(LibraryName, SetLastError = true)]
    public static extern SafeThreadHandle OpenThread(ThreadAccessFlags desiredAccess, bool inheritHandle, uint threadId);

    [DllImport(LibraryName, ExactSpelling = true, SetLastError = true)]
    public static extern SafeThreadHandle CreateRemoteThread(
    SafeProcessHandle hProcess,
    [Optional] nint lpThreadAttributes,
    nuint dwStackSize,
    nint lpStartAddress,
    [Optional] nint lpParameter,
    ThreadCreationFlags dwCreationFlags,
    [Optional] out uint lpThreadId);

    [DllImport(LibraryName, ExactSpelling = true, SetLastError = true)]
    public static extern bool GetExitCodeThread(SafeThreadHandle hHandle, out int lpdwExitCode);

    [DllImport(LibraryName, SetLastError = true)]
    public static extern WaitObjectResult WaitForSingleObject(SafeHandle hHandle, uint dwMilliseconds);

    [DllImport(LibraryName, SetLastError = true)]
    public static extern int SuspendThread(SafeThreadHandle hThread);

    [DllImport(LibraryName, SetLastError = true)]
    public static extern int ResumeThread(SafeThreadHandle hThread);
}