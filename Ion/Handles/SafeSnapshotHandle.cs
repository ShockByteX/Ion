namespace Ion.Handles;

public sealed class SafeSnapshotHandle : SafeHandle
{
    public SafeSnapshotHandle() { }
    public SafeSnapshotHandle(nint handle) : base(handle) { }
}