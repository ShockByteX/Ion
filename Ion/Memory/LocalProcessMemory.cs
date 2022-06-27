using Ion.Handles;
using Ion.Extensions;

namespace Ion.Memory;

internal sealed class LocalProcessMemory : ProcessMemory
{
    public LocalProcessMemory(SafeProcessHandle handle) : base(handle) { }

    public override byte[] Read(IntPtr address, int length) => address.ReadMemory(length);
    public override T Read<T>(IntPtr address) => address.Read<T>();
    public override int Write(IntPtr address, byte[] data) => address.WriteMemory(Handle, data);
    public override void Write<T>(IntPtr address, T value) => address.Write(Handle, value);
}