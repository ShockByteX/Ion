using Ion.Handles;
using Ion.Validation;

namespace Ion.Native.Snapshot;

internal abstract class Toolhelp32Snapshot<TEntity> where TEntity : struct
{
    public delegate bool First(SafeSnapshotHandle handle, ref TEntity entry);
    public delegate bool Next(SafeSnapshotHandle handle, ref TEntity entry);
    public delegate TEntity Create();

    private readonly int _processId;
    private readonly SnapshotFlags _flags;
    private readonly First _first;
    private readonly Next _next;
    private readonly Create _create;

    protected Toolhelp32Snapshot(int processId, SnapshotFlags flags, First first, Next next, Create create)
    {
        _processId = processId;
        _flags = flags;
        _first = first;
        _next = next;
        _create = create;
    }

    public IEnumerable<TEntity> Enumerate()
    {
        SafeSnapshotHandle? handle = null;

        try
        {
            handle = Kernel32.CreateToolhelp32Snapshot(_flags, _processId);

            Ensure.IsValid(handle);

            var entry = _create();

            if (_first(handle, ref entry))
            {
                do
                {
                    yield return entry;
                }
                while (_next(handle, ref entry));
            }
        }
        finally
        {
            handle?.Close();
            handle?.Dispose();
        }
    }
}