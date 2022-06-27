using Ion.Native;
using Ion.Validation;

namespace Ion.Modules;

internal sealed class LocalModuleReflection : IDisposable
{
    private readonly IProcessModule _localModule;
    private readonly IProcessModule _remoteModule;
    private bool _free;

    public LocalModuleReflection(IProcessModule localModule, IProcessModule remoteModule, bool free)
    {
        _localModule = localModule;
        _remoteModule = remoteModule;
        _free = free;
    }

    public IProcessFunction FindFunction(string functionName)
    {
        var address = Kernel32.GetProcAddress(_localModule.BaseAddress, functionName);

        Assert.Win32(address != IntPtr.Zero);

        var offset = (int)(address.ToInt64() - _localModule.BaseAddress.ToInt64());

        return new ProcessFunction(_remoteModule.Process.Memory, _remoteModule.BaseAddress + offset, functionName);
    }

    public void Dispose()
    {
        if (_free)
        {
            _localModule.Free();
            _free = false;
        }
    }

    public static LocalModuleReflection Attach(IProcessModule remoteModule)
    {
        IProcessModule? localModule = null;
        var loadManually = false;

        try
        {
            var currentProcess = ExtendedProcess.CurrentProcess;

            loadManually = !currentProcess.ModuleManager.TryFindByName(remoteModule.Name, out localModule);

            if (loadManually)
            {
                localModule = currentProcess.ModuleManager.Load(remoteModule.Path);
            }

            return new LocalModuleReflection(localModule!, remoteModule, loadManually);
        }
        catch
        {
            if (loadManually)
            {
                localModule?.Free();
            }

            throw;
        }
    }
}