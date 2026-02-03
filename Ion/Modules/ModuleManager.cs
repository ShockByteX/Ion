using Ion.Exceptions;
using Ion.Interop;
using Ion.Interop.Snapshot;
using Ion.Validation;
using System.Diagnostics;
using System.Text;

namespace Ion.Modules;

public interface IModuleManager
{
    IProcessModule this[string moduleName] { get; }
    IReadOnlyList<IProcessModule> Modules { get; }
    IProcessModule Load(string path);
    bool TryFindByName(string moduleName, out IProcessModule? module);
    bool TryFindByPath(string modulePath, out IProcessModule? module);
}

internal sealed class ModuleManager : IModuleManager
{
    private readonly IProcess _process;
    private readonly ModelSnapshot<IProcessModule, ModuleEntry32> _snapshot;

    public ModuleManager(IProcess process)
    {
        _process = process;
        _snapshot = new ModelSnapshot<IProcessModule, ModuleEntry32>(process.Id, SnapshotFlags.Module, Kernel32.Module32First, Kernel32.Module32Next, ModuleEntry32.Create, entry => new ProcessModule(process, entry));
    }

    public IProcessModule this[string moduleName] => FindByName(moduleName);

    public IReadOnlyList<IProcessModule> Modules => _snapshot.Get();

    public IProcessModule Load(string path)
    {
        try
        {
            Ensure.FileExists(path);

            if (_process.IsCurrent)
            {
                Ensure.Win32(Kernel32.LoadLibrary(path) != nint.Zero);
            }
            else
            {
                var kernel32Module = this[Kernel32.LibraryName];
                var loadLibraryFunction = kernel32Module["LoadLibraryW"];
                var libraryPath = Path.GetFullPath(path);
                var result = loadLibraryFunction.Execute(libraryPath, Encoding.Unicode);

                Trace.WriteLine($"LoadLibrary: 0x{result:x}");

                Ensure.That(result > 0);
            }

            Ensure.Win32(Kernel32.LoadLibrary(path) != nint.Zero);
            Ensure.That(TryFindByPath(path, out var module));

            return module!;
        }
        catch (Exception e)
        {
            throw new FailedToLoadModuleException(_process, path, e);
        }
    }

    public bool TryFindByName(string moduleName, out IProcessModule? module) => _snapshot.TryFind(x => x.Name.Equals(moduleName, StringComparison.OrdinalIgnoreCase), out module);
    public bool TryFindByPath(string modulePath, out IProcessModule? module) => _snapshot.TryFind(x => x.Path.Equals(modulePath, StringComparison.OrdinalIgnoreCase), out module);

    private IProcessModule FindByName(string moduleName)
    {
        if (!TryFindByName(moduleName, out var module))
        {
            throw new FailedToFindModuleException(_process, moduleName);
        }

        return module!;
    }
}