using Ion.Engine;
using Ion.Handles;
using Ion.Marshaling;
using Ion.Memory;
using Ion.Modules;
using Ion.Native;
using Ion.Extensions;
using Ion.Validation;

namespace Ion;

public interface IProcess : IDisposable
{
    int Id { get; }
    string Name { get; }
    SafeProcessHandle Handle { get; }
    IProcessMemory Memory { get; }
    IModuleManager ModuleManager { get; }
    bool IsCurrent { get; }

    IReadOnlyList<IProcessModule> Modules { get; }

    IMemoryPointer this[IntPtr address] { get; }
    IProcessModule this[string moduleName] { get; }

    MemoryObject<T> AllocateObject<T>();
    IntPtr ScanFirst(string pattern);
    IEnumerable<IMemoryRegion> GetMemoryRegions();
}

public sealed class ExtendedProcess : IProcess
{
    private static readonly Lazy<IProcess> LazyCurrentProcess = new(() => GetProcess(Environment.ProcessId, true));

    public static IProcess CurrentProcess => LazyCurrentProcess.Value;

    private ExtendedProcess(int processId, ProcessInfo info, SafeProcessHandle handle, bool local)
    {
        Id = processId;
        Name = info.Name;
        Handle = handle;
        Memory = local? new LocalProcessMemory(this) : new RemoteProcessMemory(this);
        ModuleManager = new ModuleManager(this);
    }

    ~ExtendedProcess() => Dispose(false);

    public int Id { get; }
    public string Name { get; }
    public SafeProcessHandle Handle { get; }
    public IProcessMemory Memory { get; }
    public IModuleManager ModuleManager { get; }

    public bool IsCurrent => Id == Environment.ProcessId;

    public IReadOnlyList<IProcessModule> Modules => ModuleManager.Modules;

    public IMemoryPointer this[IntPtr address] => new MemoryPointer(Memory, address);
    public IProcessModule this[string moduleName] => ModuleManager[moduleName];

    public MemoryObject<T> AllocateObject<T>() => MemoryObject<T>.Allocate(this, MarshalType<T>.Size);

    public IntPtr ScanFirst(string pattern)
    {
        foreach (var module in Modules)
        {
            var data = this[module.BaseAddress].Read(0, module.Size);
            var result = SignatureScanner.Scan(data, pattern, true);

            if (result.Count > 0)
            {
                return module.BaseAddress.Add(result[0]);
            }
        }

        return IntPtr.Zero;
    }

    public IEnumerable<IMemoryRegion> GetMemoryRegions()
    {
        var regions = new List<MemoryRegion>();

        Kernel32.GetSystemInfo(out var systemInfo);

        var maxAddress = (long)systemInfo.MaximumApplicationAddress;
        var currentAddress = IntPtr.Zero;

        while (Kernel32.VirtualQueryEx(Handle, currentAddress, out var info) > 0 && currentAddress.ToInt64() < maxAddress)
        {
            regions.Add(new MemoryRegion(Memory, currentAddress));
            currentAddress = info.BaseAddress.Add(info.RegionSize);
        }

        return regions;
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    private void Dispose(bool disposing)
    {
        if (disposing)
        {
            Handle.Dispose();
        }
    }

    public override string ToString() => $"Id: {Id}, Name: {Name}";

    public static IProcess GetProcess(int processId) => GetProcess(processId, Environment.ProcessId == processId);

    private static IProcess GetProcess(int processId, bool local)
    {
        var info = ProcessManager.FindById(processId);
        var handle = Kernel32.OpenProcess(ProcessAccessFlags.AllAccess, false, processId);

        Assert.IsValid(handle);

        return new ExtendedProcess(processId, info, handle, local);
    }
}