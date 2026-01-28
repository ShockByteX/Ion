namespace Ion.Handles;

public sealed class SafeProcessHandle : SafeHandle
{
    public SafeProcessHandle() { }
    public SafeProcessHandle(nint handle) : base(handle) { }
}