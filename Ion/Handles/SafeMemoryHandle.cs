namespace Ion.Handles;

internal sealed class SafeMemoryHandle : SafeHandle
{
    public SafeMemoryHandle() { }
    public SafeMemoryHandle(IntPtr handle) : base(handle) { }
}