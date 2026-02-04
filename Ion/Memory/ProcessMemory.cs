using Ion.Extensions;
using Ion.Interop;
using Ion.Marshaling;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Text;

namespace Ion.Memory;

public interface IProcessMemory
{
    IProcess Process { get; }

    IReadOnlyCollection<IMemoryRegion> GetMemoryRegions(nint minAddress, nint maxAddress);

    byte[] Read(nint address, int length);
    T Read<[DynamicallyAccessedMembers(DynamicallyAccessedMembers.Default)] T>(nint address) where T : unmanaged;
    T[] Read<[DynamicallyAccessedMembers(DynamicallyAccessedMembers.Default)] T>(nint address, int length) where T : unmanaged;
    string Read(nint address, Encoding encoding, int maxLength);
    string Read(nint address, Encoding encoding);   

    int Write(nint address, ReadOnlySpan<byte> data);
    void Write<[DynamicallyAccessedMembers(DynamicallyAccessedMembers.Default)] T>(nint address, in T value) where T : unmanaged;
    void Write<[DynamicallyAccessedMembers(DynamicallyAccessedMembers.Default)] T>(nint address, T[] values) where T : unmanaged;
    void Write(nint address, string text, Encoding encoding);
}

public abstract class ProcessMemory : IProcessMemory
{
    public const char NullTerminator = '\0';

    protected ProcessMemory(IProcess process)
    {
        Process = process;
    }

    public IProcess Process { get; }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public IReadOnlyCollection<IMemoryRegion> GetMemoryRegions(nint minAddress, nint maxAddress)
    {
        var regions = new List<MemoryRegion>();
        var maxAddressValue = maxAddress.ToInt64();

        while (Kernel32.VirtualQueryEx(Process.Handle, minAddress, out var info) > 0 && minAddress.ToInt64() < maxAddressValue)
        {
            regions.Add(new MemoryRegion(this, minAddress));
            minAddress = info.BaseAddress.Add(info.RegionSize);
        }

        return regions;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public unsafe T[] Read<[DynamicallyAccessedMembers(DynamicallyAccessedMembers.Default)] T>(nint address, int length) where T : unmanaged
    {
        var values = new T[length];
        var size = sizeof(T);

        for (var i = 0; i < length; i++)
        {
            values[i] = Read<T>(nint.Add(address, i * size));
        }

        return values;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public string Read(nint address, Encoding encoding)
    {
        var result = string.Empty;
        var offset = 0;
        char c;

        while ((c = Read<char>(nint.Add(address, offset++))) != NullTerminator)
            result += c;

        return result;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public string Read(nint address, Encoding encoding, int maxLength)
    {
        var data = Read(address, maxLength);
        var text = encoding.GetString(data);
        var ntIndex = text.IndexOf(NullTerminator);

        return ntIndex != -1 ? text.Remove(ntIndex) : text;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public unsafe void Write<[DynamicallyAccessedMembers(DynamicallyAccessedMembers.Default)] T>(nint address, T[] values) where T : unmanaged
    {
        var length = values.Length;
        var size = sizeof(T);

        for (var i = 0; i < length; i++)
            Write(nint.Add(address, i * size), values[i]);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Write(nint address, string text, Encoding encoding)
    {
        if (text[^1] != NullTerminator)
            text += NullTerminator;

        Write(address, encoding.GetBytes(text));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public abstract byte[] Read(nint address, int length);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public abstract T Read<[DynamicallyAccessedMembers(DynamicallyAccessedMembers.Default)] T>(nint address) where T : unmanaged;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public abstract int Write(nint address, ReadOnlySpan<byte> data);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public abstract void Write<[DynamicallyAccessedMembers(DynamicallyAccessedMembers.Default)] T>(nint address, in T value) where T : unmanaged;
}