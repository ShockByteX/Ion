using Ion.Extensions;
using Ion.Marshaling;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Ion.Memory;

internal sealed class RemoteProcessMemory : ProcessMemory
{
    public RemoteProcessMemory(IProcess process) : base(process) { }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override unsafe byte[] Read(nint address, int size)
    {
        var data = GC.AllocateUninitializedArray<byte>(size);
        ref var reference = ref MemoryMarshal.GetArrayDataReference(data);
        var span = MemoryMarshal.CreateSpan(ref reference, size);

        fixed (byte* pointer = span)
        {
            Process.Handle.ReadMemory(address, (nint)pointer, size);
        }

        return data;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override unsafe T Read<[DynamicallyAccessedMembers(DynamicallyAccessedMembers.Default)] T>(nint address)
    {
        T value;

        Process.Handle.ReadMemory(address, (nint)(&value), sizeof(T));

        return value;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override unsafe int Write(nint address, ReadOnlySpan<byte> data)
    {
        ref var reference = ref MemoryMarshal.GetReference<byte>(data);
        var pointer = (nint)Unsafe.AsPointer(ref reference);

        return Process.Handle.WriteMemory(address, pointer, data.Length);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override unsafe void Write<[DynamicallyAccessedMembers(DynamicallyAccessedMembers.Default)] T>(nint address, in T value)
    {
        ref var reference = ref Unsafe.AsRef(in value);
        var pointer = (nint)Unsafe.AsPointer(ref reference);

        Process.Handle.WriteMemory(address, pointer, sizeof(T));
    }
}