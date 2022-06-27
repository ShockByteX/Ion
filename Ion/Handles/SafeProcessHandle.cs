namespace Ion.Handles;

public sealed class SafeProcessHandle : SafeHandle
{
    public SafeProcessHandle() { }
    public SafeProcessHandle(IntPtr handle) : base(handle) { }
}