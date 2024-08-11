namespace Ion.Engine;

public static unsafe class SignatureScanner
{
    public static IReadOnlyList<int> Scan(byte[] data, string pattern, int startOffset = 0) => Scan(data, pattern, startOffset, firstOnly: false);
    public static int ScanFirst(byte[] data, string pattern, int startOffset = 0) => Scan(data, pattern, startOffset, firstOnly: true).Single();

    public static IReadOnlyList<int> Scan(byte[] data, string pattern, int startOffset, bool firstOnly)
    {
        var signature = BytesConverter.ToBytes(pattern, out var unknownByte);
        return Scan(data, signature, unknownByte, startOffset, firstOnly);
    }

    public static int ScanFirst(byte[] data, byte[] signature, byte unknownByte) => Scan(data, signature, unknownByte).Single();

    public static IReadOnlyList<int> Scan(byte[] data, byte[] signature, byte unknownByte, int startOffset = 0, bool firstOnly = true)
    {
        var endPoint = data.Length - signature.Length - 1;
        var sigByte = signature[0];
        var sigLength = signature.Length;

        var offsets = new List<int>();

        fixed (byte* ptrData = data, ptrSignature = signature)
        {
            for (var i = startOffset; i < endPoint; i++)
            {
                if (!sigByte.Equals(ptrData[i])) continue;
                if (!SequenceEquals(ptrData, ptrSignature, unknownByte, i, sigLength)) continue;

                offsets.Add(i);

                if (firstOnly)
                {
                    return offsets;
                }
            }
        }

        return offsets;
    }

    private static bool SequenceEquals(byte* ptrData, byte* ptrSignature, byte unknownByte, int index, int length)
    {
        for (var i = 0; i < length; i++)
        {
            if (ptrSignature[i] == unknownByte) continue;
            if (ptrSignature[i] != ptrData[index + i]) return false;
        }

        return true;
    }
}