using Ion.Extensions;
using Ion.Interop;
using System.Diagnostics.CodeAnalysis;

namespace Ion.Memory;

internal sealed class LocalProcessMemory : ProcessMemory
{
    public LocalProcessMemory(IProcess process) : base(process) { }

    public override byte[] Read(nint address, int length) => address.ReadMemory(length);
    public override T Read<[DynamicallyAccessedMembers(DynamicallyAccessedMembers.Default)] T>(nint address) => address.Read<T>();
    public override int Write(nint address, ReadOnlySpan<byte> data) => address.WriteMemory(Process.Handle, data);
    public override void Write<[DynamicallyAccessedMembers(DynamicallyAccessedMembers.Default)] T>(nint address, in T value) => address.Write(Process.Handle, value);
}