using Ion.Native;
using Microsoft.Win32.SafeHandles;

namespace Ion.Handles;

public abstract class SafeHandle : SafeHandleZeroOrMinusOneIsInvalid
{
    protected SafeHandle() : base(true) { }

    protected SafeHandle(nint handle) : base(true)
    {
        SetHandle(handle);
    }

    protected override bool ReleaseHandle()
    {
        return handle != nint.Zero && Kernel32.CloseHandle(handle);
    }
}