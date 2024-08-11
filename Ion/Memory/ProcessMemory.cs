using System.Text;
using Ion.Extensions;
using Ion.Marshaling;
using Ion.Native;

namespace Ion.Memory;

public interface IProcessMemory
{
    IProcess Process { get; }

    IReadOnlyCollection<IMemoryRegion> GetMemoryRegions(IntPtr minAddress, IntPtr maxAddress);

    byte[] Read(IntPtr address, int length);
    T Read<T>(IntPtr address);
    T[] Read<T>(IntPtr address, int length);
    string Read(IntPtr address, Encoding encoding, int maxLength);
    string Read(IntPtr address, Encoding encoding);

    int Write(IntPtr address, byte[] data);
    void Write<T>(IntPtr address, T value);
    void Write<T>(IntPtr address, T[] values);
    void Write(IntPtr address, string text, Encoding encoding);
}

public abstract class ProcessMemory : IProcessMemory
{
    public const char NullTerminator = '\0';

    protected ProcessMemory(IProcess process)
    {
        Process = process;
    }

    public IProcess Process { get; }

    public IReadOnlyCollection<IMemoryRegion> GetMemoryRegions(IntPtr minAddress, IntPtr maxAddress)
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

    public T[] Read<T>(IntPtr address, int length)
    {
        var values = new T[length];
        var size = MarshalType<T>.Size;

        for (var i = 0; i < length; i++)
        {
            values[i] = Read<T>(IntPtr.Add(address, i * size));
        }

        return values;
    }

    public string Read(IntPtr address, Encoding encoding)
    {
        var result = string.Empty;
        var offset = 0;
        char c;

        while ((c = Read<char>(IntPtr.Add(address, offset++))) != NullTerminator)
        {
            result += c;
        }

        return result;
    }

    public string Read(IntPtr address, Encoding encoding, int maxLength)
    {
        var data = Read(address, maxLength);
        var text = encoding.GetString(data);
        var ntIndex = text.IndexOf(NullTerminator);

        return ntIndex != -1 ? text.Remove(ntIndex) : text;
    }

    public void Write<T>(IntPtr address, T[] values)
    {
        var length = values.Length;
        var size = MarshalType<T>.Size;

        for (var i = 0; i < length; i++)
        {
            Write(IntPtr.Add(address, i * size), values[i]);
        }
    }

    public void Write(IntPtr address, string text, Encoding encoding)
    {
        if (text[^1] != NullTerminator)
        {
            text += NullTerminator;
        }

        Write(address, encoding.GetBytes(text));
    }

    public abstract byte[] Read(IntPtr address, int length);
    public abstract T Read<T>(IntPtr address);
    public abstract int Write(IntPtr address, byte[] data);
    public abstract void Write<T>(IntPtr address, T value);
}