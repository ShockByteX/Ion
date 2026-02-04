using Ion.Interop.Handles;

namespace Ion.Modules;

internal sealed class ModuleFunction : IEquatable<ModuleFunction>
{
    public ModuleFunction(SafeProcessHandle processHandle, string modulePath, string functionName)
    {
        ModulePath = modulePath;
        FunctionName = functionName;
        ProcessHandle = processHandle;
    }

    public SafeProcessHandle ProcessHandle { get; }
    public string ModulePath { get; }
    public string FunctionName { get; }

    public bool Equals(ModuleFunction? other) => other is not null
                                                 && other.ProcessHandle == ProcessHandle
                                                 && other.ModulePath.Equals(ModulePath, StringComparison.OrdinalIgnoreCase)
                                                 && other.FunctionName.Equals(FunctionName, StringComparison.OrdinalIgnoreCase);
    public override bool Equals(object? obj) => obj is ModuleFunction other && Equals(other);
    public override int GetHashCode() => HashCode.Combine(ProcessHandle, ModulePath, FunctionName);

    public override string ToString() => $"ProcessHandle: {ProcessHandle}, ModulePath: {ModulePath}, FunctionName: {FunctionName}";
}