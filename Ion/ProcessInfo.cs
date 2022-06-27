using Ion.Native;

namespace Ion;

public sealed class ProcessInfo
{
    internal ProcessInfo(ProcessEntry32 entry)
    {
        Id = entry.ProcessId;
        ParentId = entry.ParentProcessId;
        Name = entry.ProcessName;
    }

    public int Id { get; }
    public int ParentId { get; }
    public string Name { get; }
}