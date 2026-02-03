using System.Text;
using Ion.Extensions;
using Ion.Marshaling;
using Ion.Interop;

namespace Ion.Memory;

public interface IProcessMemory
{
    IProcess Process { get; }

    IReadOnlyCollection<IMemoryRegion> GetMemoryRegions(nint minAddress, nint maxAddress);

    byte[] Read(nint address, int length);
    T Read<T>(nint address) where T : struct;
    T[] Read<T>(nint address, int length) where T : struct;
    string Read(nint address, Encoding encoding, int maxLength);
    string Read(nint address, Encoding encoding);

    int Write(nint address, byte[] data);
    void Write<T>(nint address, T value) where T : struct;
    void Write<T>(nint address, T[] values) where T : struct;
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

    public T[] Read<T>(nint address, int length) where T : struct
    {
        var values = new T[length];
        var size = MarshalType<T>.Size;

        for (var i = 0; i < length; i++)
        {
            values[i] = Read<T>(nint.Add(address, i * size));
        }

        return values;
    }

    public string Read(nint address, Encoding encoding)
    {
        var result = string.Empty;
        var offset = 0;
        char c;

        while ((c = Read<char>(nint.Add(address, offset++))) != NullTerminator)
        {
            result += c;
        }

        return result;
    }

    public string Read(nint address, Encoding encoding, int maxLength)
    {
        var data = Read(address, maxLength);
        var text = encoding.GetString(data);
        var ntIndex = text.IndexOf(NullTerminator);

        return ntIndex != -1 ? text.Remove(ntIndex) : text;
    }

    public void Write<T>(nint address, T[] values) where T : struct
    {
        var length = values.Length;
        var size = MarshalType<T>.Size;

        for (var i = 0; i < length; i++)
        {
            Write(nint.Add(address, i * size), values[i]);
        }
    }

    public void Write(nint address, string text, Encoding encoding)
    {
        if (text[^1] != NullTerminator)
        {
            text += NullTerminator;
        }

        Write(address, encoding.GetBytes(text));
    }

    public abstract byte[] Read(nint address, int length);
    public abstract T Read<T>(nint address) where T : struct;
    public abstract int Write(nint address, byte[] data);
    public abstract void Write<T>(nint address, T value) where T : struct;
}