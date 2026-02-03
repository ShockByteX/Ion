namespace Ion.Interop.Snapshot;

internal sealed class ModelSnapshot<TModel, TEntity> : Toolhelp32Snapshot<TEntity> where TEntity : struct
{
    private readonly Func<TEntity, TModel> _fetch;

    public ModelSnapshot(int processId, SnapshotFlags flags, First first, Next next, Create create, Func<TEntity, TModel> fetch) : base(processId, flags, first, next, create)
    {
        _fetch = fetch;
    }

    public IReadOnlyList<TModel> Get() => Fetch().ToArray();

    public bool TryFind(Func<TModel, bool> predicate, out TModel? module)
    {
        module = Fetch().FirstOrDefault(predicate);
        return module != null;
    }

    private IEnumerable<TModel> Fetch() => Enumerate().Select(_fetch);
}