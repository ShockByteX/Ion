using Ion.Exceptions;
using Ion.Extensions;
using Ion.Memory;
using Ion.Native;
using Ion.Validation;

namespace Ion.Modules;

public interface IProcessModule
{
    IProcess Process { get; }
    string Name { get; }
    string Path { get; }
    int Size { get; }
    IntPtr BaseAddress { get; }
    IProcessFunction this[string functionName] { get; }

    IMemoryDump Dump();
    IReadOnlyCollection<IMemoryRegion> GetMemoryRegions();

    void Free();
}

internal sealed class ProcessModule : IProcessModule
{
    private static readonly Dictionary<ModuleFunction, IProcessFunction> Functions = new();

    public ProcessModule(IProcess process, ModuleEntry32 entry)
    {
        Process = process;
        Name = entry.Name.ToLower();
        Path = entry.FileName.ToLower();
        Size = entry.BaseSize;
        BaseAddress = entry.BaseAddress;
    }

    public IProcess Process { get; }
    public string Name { get; }
    public string Path { get; }
    public int Size { get; }
    public IntPtr BaseAddress { get; }

    public IProcessFunction this[string functionName] => FindFunction(functionName);

    public IMemoryDump Dump() => MemoryDump.Dump(Process, BaseAddress, Size);

    public IReadOnlyCollection<IMemoryRegion> GetMemoryRegions()
    {
        return Process.Memory.GetMemoryRegions(BaseAddress, BaseAddress.Add(Size));
    }

    public IProcessFunction FindFunction(string functionName)
    {
        LocalModuleReflection? reflection = null;

        try
        {
            Assert.NotNullOrEmpty(functionName);

            var moduleFunction = new ModuleFunction(Process.Handle, Path, functionName);

            if (!Functions.TryGetValue(moduleFunction, out var function))
            {
                reflection = LocalModuleReflection.Attach(this);
                function = reflection.FindFunction(functionName);
                Functions.Add(moduleFunction, function);
            }

            return function;
        }
        catch (Exception e)
        {
            throw new FailedToFindFunctionException(Process, Path, functionName, e);
        }
        finally
        {
            reflection?.Dispose();
        }
    }

    public void Free()
    {
        if (Process.IsCurrent)
        {
            Assert.Win32(Kernel32.FreeLibrary(BaseAddress));
            return;
        }
    }
}