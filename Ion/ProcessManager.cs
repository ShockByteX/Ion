using Ion.Exceptions;
using Ion.Native;
using Ion.Native.Snapshot;

namespace Ion;

public static class ProcessManager
{
    private static readonly ModelSnapshot<ProcessInfo, ProcessEntry32> Snapshot;

    static ProcessManager()
    {
        Snapshot = new ModelSnapshot<ProcessInfo, ProcessEntry32>(0, SnapshotFlags.Process, Kernel32.Process32First, Kernel32.Process32Next, ProcessEntry32.Create, entry => new ProcessInfo(entry));
    }

    public static IReadOnlyList<ProcessInfo> Processes => Snapshot.Get();

    public static ProcessInfo FindById(int processId)
    {
        if (!TryFindById(processId, out var process))
        {
            throw new FailedToFindProcessException(processId);
        }

        return process!;
    }

    public static bool TryFindById(int processId, out ProcessInfo? process) => Snapshot.TryFind(x => x.Id == processId, out process);
    public static bool TryFindByName(string processName, out ProcessInfo? process) => Snapshot.TryFind(x => x.Name.Equals(processName, StringComparison.OrdinalIgnoreCase), out process);
}