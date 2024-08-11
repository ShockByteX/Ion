using Ion.Marshaling;
using Ion.Validation;
using System.Text;

namespace Ion.PE32;

internal class PortableExecutableBinary
{
    private const char NullTerminator = '\0';

    private readonly byte[] _data;

    private PortableExecutableBinary(byte[] data)
    {
        _data = data;
    }

    public byte[] Read(int offset, int length) => _data[offset..(offset + length)];

    public T Read<T>(int offset) where T : struct => MarshalType<T>.Convert(Read(offset, MarshalType<T>.Size));

    public string Read(int offset, Encoding encoding, int maxLength)
    {
        var data = Read(offset, maxLength);
        var text = encoding.GetString(data);
        var ntIndex = text.IndexOf(NullTerminator);

        return ntIndex != -1 ? text.Remove(ntIndex) : text;
    }

    public static PortableExecutableBinary Load(string filePath)
    {
        Assert.FileExists(filePath);

        var data = File.ReadAllBytes(filePath);

        return Load(data);
    }

    public static PortableExecutableBinary Load(byte[] data) => new PortableExecutableBinary(data);
}