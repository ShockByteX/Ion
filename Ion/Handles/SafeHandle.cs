using Ion.Native;
using Microsoft.Win32.SafeHandles;

namespace Ion.Handles;

public abstract class SafeHandle : SafeHandleZeroOrMinusOneIsInvalid
{
    protected SafeHandle() : base(true) { }

    protected SafeHandle(IntPtr handle) : base(true)
    {
        SetHandle(handle);
    }

    protected override bool ReleaseHandle()
    {
        return handle != IntPtr.Zero && Kernel32.CloseHandle(handle);
    }
}