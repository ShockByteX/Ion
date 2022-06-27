using System.Runtime.InteropServices;
using Ion.Memory;
using Ion.Extensions;
using Ion.Validation;

namespace Ion.Modules;

public interface IProcessFunction : IEquatable<IProcessFunction>
{
    IntPtr Address { get; }
    string Name { get; }
    T GetDelegate<T>();
}

internal sealed class ProcessFunction : MemoryPointer, IEquatable<ProcessFunction>, IProcessFunction
{
    public ProcessFunction(IProcessMemory memory, IntPtr address, string name) : base(memory, address)
    {
        Name = name;
    }

    public string Name { get; }

    public T GetDelegate<T>()
    {
        Assert.IsValid(Address);

        return Marshal.GetDelegateForFunctionPointer<T>(Address);
    }

    public bool Equals(ProcessFunction? other) => Equals((IProcessFunction?)other);
    public bool Equals(IProcessFunction? other) => other is not null && Equals((IMemoryPointer)this) && other.Name.Equals(Name, StringComparison.OrdinalIgnoreCase);
    public override bool Equals(object? obj) => obj is IProcessFunction function && Equals(function);
    public override int GetHashCode() => base.GetHashCode() ^ Name.GetHashCode();

    public override string ToString() => $"Address: {Address.ToHexadecimalString()}, Name: {Name}";
}