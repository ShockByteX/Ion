namespace Ion.Interop.Handles;

internal sealed class SafeMemoryHandle : SafeHandle
{
    public SafeMemoryHandle() { }
    public SafeMemoryHandle(nint handle) : base(handle) { }
}