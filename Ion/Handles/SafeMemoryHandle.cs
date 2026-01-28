namespace Ion.Handles;

internal sealed class SafeMemoryHandle : SafeHandle
{
    public SafeMemoryHandle() { }
    public SafeMemoryHandle(nint handle) : base(handle) { }
}