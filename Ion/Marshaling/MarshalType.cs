using System.Runtime.InteropServices;
using Ion.Validation;

namespace Ion.Marshaling;

public static class MarshalType
{
    public static byte[] Convert(object value)
    {
        var type = value.GetType();

        if (type.IsEnum)
        {
            type = type.GetEnumUnderlyingType();
        }

        var size = Marshal.SizeOf(value);
        var typeCode = Type.GetTypeCode(type);

        return Convert(value, type, typeCode, size);
    }

    public static object Convert(Memory<byte> data, Type type)
    {
        if (type.IsEnum)
        {
            type = type.GetEnumUnderlyingType();
        }

        var typeCode = Type.GetTypeCode(type);

        return Convert(data, type, typeCode);
    }

    internal static byte[] Convert(object value, Type type, TypeCode typeCode, int size)
    {
        switch (typeCode)
        {
            case TypeCode.Object: break;
            case TypeCode.Boolean: return Convert((bool)value, 1);
            case TypeCode.Byte: return new[] { (byte)value };
            case TypeCode.SByte: return new[] { (byte)(sbyte)value };
            case TypeCode.Int16: return Convert((short)value, 2);
            case TypeCode.UInt16: return Convert((ushort)value, 2);
            case TypeCode.Int32: return Convert((int)value, 4);
            case TypeCode.UInt32: return Convert((uint)value, 4);
            case TypeCode.Int64: return Convert((long)value, 8);
            case TypeCode.UInt64: return Convert((ulong)value, 8);
            case TypeCode.Single: return Convert((float)value, 4);
            case TypeCode.Double: return Convert((double)value, 8);
            case TypeCode.Decimal: return Convert((decimal)value, 16);
            case TypeCode.DateTime: return Convert((DateTime)value, 8);
            default: throw new InvalidCastException("Conversion not support");
        }

        var data = new byte[size];

        unsafe
        {
            fixed (byte* pData = data)
            {
                Marshal.StructureToPtr(value, (IntPtr)pData, false);
            }
        }

        return data;
    }

    internal static object Convert(Memory<byte> data, Type type, TypeCode typeCode)
    {
        switch (typeCode)
        {
            case TypeCode.Object: break;
            case TypeCode.Boolean: return MemoryMarshal.Read<bool>(data.Span);
            case TypeCode.Byte:
            case TypeCode.SByte: return data.Span[0];
            case TypeCode.Int16: return MemoryMarshal.Read<short>(data.Span);
            case TypeCode.UInt16: return MemoryMarshal.Read<ushort>(data.Span);
            case TypeCode.Int32: return MemoryMarshal.Read<int>(data.Span);
            case TypeCode.UInt32: return MemoryMarshal.Read<uint>(data.Span);
            case TypeCode.Int64: return MemoryMarshal.Read<long>(data.Span);
            case TypeCode.UInt64: return MemoryMarshal.Read<ulong>(data.Span);
            case TypeCode.Single: return MemoryMarshal.Read<float>(data.Span);
            case TypeCode.Double: return MemoryMarshal.Read<double>(data.Span);
            case TypeCode.Decimal: return MemoryMarshal.Read<decimal>(data.Span);
            case TypeCode.DateTime: return MemoryMarshal.Read<DateTime>(data.Span);
            default: throw new InvalidCastException("Conversion not support");
        }

        unsafe
        {
            fixed (byte* pData = data.Span)
            {
                return Assert.NotNull(Marshal.PtrToStructure((IntPtr)pData, type));
            }
        }
    }

    private static byte[] Convert<TValueType>(TValueType value, int size) where TValueType : struct
    {
        var data = new byte[size];

        MemoryMarshal.Write(data, ref value);

        return data;
    }
}

public static class MarshalType<T>
{
    public static readonly Type RealType;
    public static readonly int Size;
    public static readonly TypeCode TypeCode;

    static MarshalType()
    {
        RealType = typeof(T);

        if (RealType.IsEnum)
        {
            RealType = RealType.GetEnumUnderlyingType();
        }

        Size = Marshal.SizeOf(RealType);
        TypeCode = Type.GetTypeCode(RealType);
    }

    public static byte[] Convert(T? value)
    {
        return MarshalType.Convert(Assert.NotNull(value as object, nameof(value)), RealType, TypeCode, Size);
    }

    public static T Convert(Memory<byte> data)
    {
        return (T)MarshalType.Convert(data, RealType, TypeCode);
    }
}