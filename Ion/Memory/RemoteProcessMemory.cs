using Ion.Marshaling;
using Ion.Extensions;

namespace Ion.Memory;

internal sealed class RemoteProcessMemory : ProcessMemory
{
    public RemoteProcessMemory(IProcess process) : base(process) { }

    public override byte[] Read(nint address, int length) => Process.Handle.ReadMemory(address, length);

    public override T Read<T>(nint address) => MarshalType<T>.ToValue(Read(address, MarshalType<T>.Size));

    public override int Write(nint address, byte[] data) => Process.Handle.WriteMemory(address, data);

    public override void Write<T>(nint address, T value) => Write(address, MarshalType<T>.ToBytes(value));
}