namespace Ion.Handles;

public sealed class SafeSnapshotHandle : SafeHandle
{
    public SafeSnapshotHandle() { }
    public SafeSnapshotHandle(IntPtr handle) : base(handle) { }
}