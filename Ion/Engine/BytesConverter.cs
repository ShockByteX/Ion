namespace Ion.Engine;

public static class BytesConverter
{
    public static readonly string[] BytesArray = new string[byte.MaxValue];

    static BytesConverter()
    {
        for (var i = 0; i < BytesArray.Length; i++)
        {
            var strByte = i.ToString("X");

            if (strByte.Length == 1)
            {
                BytesArray[i] = $"0{strByte}";
            }
            else
            {
                BytesArray[i] = strByte;
            }
        }
    }

    public static byte[] ToBytes(string pattern, out byte unknownByte)
    {
        var arrBytes = pattern.Split(' ');
        unknownByte = 0x00;

        for (byte i = 0; i < BytesArray.Length; i++)
        {
            if (!arrBytes.Contains(BytesArray[i]))
            {
                unknownByte = i;
                break;
            }
        }

        var signature = new byte[arrBytes.Length];

        for (var i = 0; i < arrBytes.Length; i++)
        {
            if (arrBytes[i] == "?")
            {
                signature[i] = unknownByte;
            }
            else
            {
                signature[i] = Convert.ToByte(arrBytes[i], 16);
            }
        }

        return signature;
    }
}