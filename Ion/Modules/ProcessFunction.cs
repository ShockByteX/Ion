using System.Runtime.InteropServices;
using Ion.Memory;
using Ion.Extensions;
using Ion.Validation;
using Ion.Native;
using System.Text;

namespace Ion.Modules;

public interface IProcessFunction : IEquatable<IProcessFunction>
{
    nint Address { get; }
    string Name { get; }
    T GetDelegate<T>();
    int Execute(nint parameterPointer);
    int Execute(string parameter, Encoding encoding);
}

internal sealed class ProcessFunction(IProcess process, nint address, string name) 
    : MemoryPointer(process.Memory, address), IEquatable<ProcessFunction>, IProcessFunction
{
    public string Name { get; } = name;

    public T GetDelegate<T>()
    {
        Ensure.IsValid(Address);

        return Marshal.GetDelegateForFunctionPointer<T>(Address);
    }

    public int Execute(nint parameterPointer)
    {
        using var thread = process.CreateThread(Address, parameterPointer, ThreadCreationFlags.ThreadCreateRunImmediately);
        return thread.Wait();
    }

    public int Execute(string parameter, Encoding encoding)
    {
        using var allocatedMemory = process.AllocateMemory(encoding.GetByteCount(parameter) + 1, MemoryAllocationFlags.Commit | MemoryAllocationFlags.Reserve, MemoryProtectionFlags.ReadWrite);
        allocatedMemory.Write(0, parameter, encoding);
        return Execute(allocatedMemory.Address);
    }

    public bool Equals(ProcessFunction? other) => Equals((IProcessFunction?)other);
    public bool Equals(IProcessFunction? other) => other is not null && Equals((IMemoryPointer)this) && other.Name.Equals(Name, StringComparison.OrdinalIgnoreCase);
    public override bool Equals(object? obj) => obj is IProcessFunction function && Equals(function);
    public override int GetHashCode() => base.GetHashCode() ^ Name.GetHashCode();

    public override string ToString() => $"Address: {Address.ToHexadecimalString()}, Name: {Name}";
}