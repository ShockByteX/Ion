using Ion.Engine;
using Ion.Handles;
using Ion.Marshaling;
using Ion.Memory;
using Ion.Modules;
using Ion.Native;
using Ion.Extensions;
using Ion.Validation;
using Ion.Threading;

namespace Ion;

public interface IProcess
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
    IAllocatedMemory AllocateMemory(int size, MemoryAllocationFlags allocation, MemoryProtectionFlags protection);
    IProcessThreadDisposable CreateThread(IntPtr functionPointer, IntPtr parameterPointer, ThreadCreationFlags flags);
    IntPtr ScanFirst(string pattern);
    IReadOnlyCollection<IMemoryRegion> GetMemoryRegions();
}

public interface IProcessDisposable : IProcess, IDisposable { }

public sealed class ExtendedProcess : IProcessDisposable
{
    private static readonly Lazy<IProcess> LazyCurrentProcess = new(() => GetProcess(Environment.ProcessId, false));

    public static IProcess CurrentProcess => LazyCurrentProcess.Value;

    private ExtendedProcess(int processId, ProcessInfo info, SafeProcessHandle handle, bool local)
    {
        Id = processId;
        Name = info.Name;
        Handle = handle;
        Memory = local ? new LocalProcessMemory(this) : new RemoteProcessMemory(this);
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
    public IAllocatedMemory AllocateMemory(int size, MemoryAllocationFlags allocation, MemoryProtectionFlags protection) => AllocatedMemory.Allocate(Memory, size, allocation, protection);

    public IProcessThreadDisposable CreateThread(IntPtr functionPointer, IntPtr parameterPointer, ThreadCreationFlags flags)
    {
        return ProcessThread.Create(Handle, functionPointer, parameterPointer, flags);
    }

    public IntPtr ScanFirst(string pattern)
    {
        foreach (var module in Modules)
        {
            var data = this[module.BaseAddress].Read(0, module.Size);
            var result = SignatureScanner.Scan(data, pattern, 0, true);

            if (result.Count > 0)
            {
                return module.BaseAddress.Add(result[0]);
            }
        }

        return IntPtr.Zero;
    }

    public IReadOnlyCollection<IMemoryRegion> GetMemoryRegions()
    {
        Kernel32.GetSystemInfo(out var systemInfo);
        return Memory.GetMemoryRegions(IntPtr.Zero, systemInfo.MaximumApplicationAddress);
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

    public static IProcessDisposable GetProcess(int processId) => GetProcess(processId, Environment.ProcessId == processId);

    private static IProcessDisposable GetProcess(int processId, bool local)
    {
        var info = ProcessManager.FindById(processId);
        var handle = Kernel32.OpenProcess(ProcessAccessFlags.AllAccess, false, processId);

        Ensure.IsValid(handle);

        return new ExtendedProcess(processId, info, handle, local);
    }
}