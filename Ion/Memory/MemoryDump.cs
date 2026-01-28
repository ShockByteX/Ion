using Ion.Engine;

namespace Ion.Memory;

public interface IMemoryDump
{
    public nint Address { get; }

    byte[] GetData();
    int ScanFirst(byte[] signature, byte unknownByte, int offset, int extra, bool relative);
    int ScanFirst(string pattern, int offset, int extra, bool relative, int startOffset = 0);
}

internal sealed class MemoryDump : IMemoryDump
{
    private readonly IProcess _process;
    private readonly byte[] _data;

    public MemoryDump(IProcess process, nint address, byte[] data)
    {
        _process = process;
        Address = address;
        _data = data;
    }

    public nint Address { get; }

    public byte[] GetData() => _data.ToArray();

    public int ScanFirst(byte[] signature, byte unknownByte, int offset, int extra, bool relative)
    {
        var result = SignatureScanner.ScanFirst(_data, signature, unknownByte);
        var address = nint.Add(Address, result + offset);

        if (relative)
        {
            address = _process[address].Read<nint>(0);
        }

        return address.ToInt32() + extra;
    }

    public int ScanFirst(string pattern, int offset, int extra, bool relative, int startOffset = 0)
    {
        var result = SignatureScanner.ScanFirst(_data, pattern, startOffset) + offset;

        if (relative)
        {
            result += _process[Address].Read<int>(result) + 4;
        }

        return result + extra;
    }

    public static IMemoryDump Dump(IProcess process, nint address, int size)
    {
        var data = process[address].Read(0, size);
        return new MemoryDump(process, address, data);
    }
}