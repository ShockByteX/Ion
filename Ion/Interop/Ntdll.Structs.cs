using Ion.Interop.Primitives;
using Ion.Validation;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Ion.Interop;

[StructLayout(LayoutKind.Sequential)]
internal unsafe struct ObjectAttributes
{
    public static readonly int Size = Marshal.SizeOf<ObjectAttributes>();

    public int Length;
    public nint RootDirectory;
    public UnicodeString* ObjectName;
    public ObjectAttributeFlags Attributes;
    public nint SecurityDescriptor;
    public nint SecurityQualityOfService;
}

[StructLayout(LayoutKind.Sequential)]
internal struct IoStatusBlock
{
    public uint Status;
    public nint Information;
}

[StructLayout(LayoutKind.Sequential)]
public struct UnicodeString
{
    public ushort Length;
    public ushort MaximumLength;
    public PWSTR Buffer;
}

public sealed class SafeUnicodeString : IDisposable
{
    private static readonly ushort MaxUnicodeLength = (ushort.MaxValue - 2) >> 1;

    private UnicodeString _value;

    private SafeUnicodeString(UnicodeString value) => _value = value;

    public ref UnicodeString Reference => ref _value;

    public void Dispose()
    {
        if (!_value.Buffer.IsNull)
        {
            Marshal.FreeHGlobal(_value.Buffer);
            _value = default;
        }
    }

    public static SafeUnicodeString Allocate(string text)
    {
        Ensure.That(text.Length <= MaxUnicodeLength);

        var length = (ushort)(text.Length << 1);

        return new SafeUnicodeString(new UnicodeString
        {
            Length = length,
            MaximumLength = (ushort)(length + 2),
            Buffer = new PWSTR(Marshal.StringToHGlobalUni(text))
        });
    }

    public static implicit operator UnicodeString(SafeUnicodeString str) => str._value;
}