using Ion.Handles;
using Ion.Marshaling;
using Ion.Extensions;

namespace Ion.Memory;

internal sealed class RemoteProcessMemory : ProcessMemory
{
    public RemoteProcessMemory(SafeProcessHandle handle) : base(handle) { }

    public override byte[] Read(IntPtr address, int length) => Handle.ReadMemory(address, length);

    public override T Read<T>(IntPtr address) => MarshalType<T>.Convert(Read(address, MarshalType<T>.Size));

    public override int Write(IntPtr address, byte[] data) => Handle.WriteMemory(address, data);

    public override void Write<T>(IntPtr address, T value) => Write(address, MarshalType<T>.Convert(value));
}