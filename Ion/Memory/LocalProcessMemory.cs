using Ion.Interop;
using Ion.Extensions;

namespace Ion.Memory;

internal sealed class LocalProcessMemory : ProcessMemory
{
    public LocalProcessMemory(IProcess process) : base(process) { }

    public override byte[] Read(nint address, int length) => address.ReadMemory(length);
    public override T Read<T>(nint address) => address.Read<T>();
    public override int Write(nint address, byte[] data) => address.WriteMemory(Process.Handle, data);
    public override void Write<T>(nint address, T value) => address.Write(Process.Handle, value);
}