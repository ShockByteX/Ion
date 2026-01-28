using Ion.Validation;

namespace Ion.Engine;

public static unsafe class SignatureScanner
{
    public static IReadOnlyList<int> Scan(byte[] data, string pattern, int from = 0) => Scan(data, Signature.Parse(pattern), from, firstOnly: false);
    public static IReadOnlyList<int> Scan(byte[] data, byte[] pattern, int from = 0) => Scan(data, Signature.Create(pattern), from, firstOnly: false);

    public static int ScanFirst(byte[] data, string pattern, int from = 0) => Scan(data, Signature.Parse(pattern), from, firstOnly: true).Single();
    public static int ScanFirst(byte[] data, byte[] pattern, int from = 0) => Scan(data, Signature.Create(pattern), from, firstOnly: true).Single();

    public static bool TryScan(byte[] data, string pattern, out int offset)
    {
        var signature = Signature.Parse(pattern);

        offset = Scan(data, signature, firstOnly: true).FirstOrDefault(-1);

        return offset > -1;
    }

    private static IReadOnlyList<int> Scan(byte[] data, Signature signature, int from = 0, bool firstOnly = true)
    {
        var pattern = signature.Pattern;

        var to = data.Length - pattern.Length;
        var offsets = firstOnly ? null : new List<int>();

        fixed (byte* pData = data, pPattern = pattern)
        {
            for (var i = from; i <= to; i++)
            {
                if (!SequenceEquals(pData + i, pPattern, pattern.Length, signature.Wildcard)) continue;

                if (firstOnly)
                    return [i];

                offsets!.Add(i);
            }
        }

        return offsets ?? (IReadOnlyList<int>)[];
    }

    private static bool SequenceEquals(byte* data, byte* pattern, int length, byte wildcard)
    {
        for (var i = 0; i < length; i++)
        {
            if (pattern[i] != wildcard && pattern[i] != data[i])
                return false;
        }

        return true;
    }

    public readonly struct Signature
    {
        private const char Delimiter = ' ';
        private const char Unknown = '?';

        private Signature(byte[] pattern, byte wildcard)
        {
            Pattern = pattern;
            Wildcard = wildcard;
        }

        public readonly byte[] Pattern;
        public readonly byte Wildcard;

        public static Signature Parse(string pattern)
        {
            var parts = pattern.Split(Delimiter, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            var wildcard = FindWildcard(parts);
            var compiledPattern = new byte[parts.Length];

            for (var i = 0; i < compiledPattern.Length; i++)
            {
                var part = parts[i];

                compiledPattern[i] = IsWildcard(part)
                    ? wildcard
                    : Convert.ToByte(part, 16);
            }

            return new Signature(compiledPattern, wildcard);
        }

        public static Signature Create(byte[] pattern)
        {
            var knownBytes = new bool[256];

            for (var i = 0; i < pattern.Length; i++)
                knownBytes[pattern[i]] = true;

            var wildcard = Array.IndexOf(knownBytes, false);

            Ensure.That(wildcard != -1, () => "Cannot create signature from pattern with all possible byte values.");

            return new Signature(pattern, (byte)wildcard);
        }

        private static byte FindWildcard(string[] parts)
        {
            var knownBytes = new bool[256];

            foreach (var part in parts)
            {
                if (!IsWildcard(part))
                    knownBytes[Convert.ToByte(part, 16)] = true;
            }

            return (byte)Array.IndexOf(knownBytes, false);
        }

        private static bool IsWildcard(string part) => part.Length is 1 && part[0] == Unknown;
    }
}