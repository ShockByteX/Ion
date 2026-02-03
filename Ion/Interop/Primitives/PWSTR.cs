using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Ion.Interop.Primitives;

public unsafe readonly partial struct PWSTR : IEquatable<PWSTR>
{
    public readonly char* Value;

    public PWSTR(char* value) => Value = value;
    public PWSTR(nint value) : this(unchecked((char*)value)) { }

    public bool IsNull => Value is null;

    public int Length
    {
        get
        {
            char* current = Value;

            if (current is null)
                return 0;

            while (*current != '\0')
                current++;

            return checked((int)(current - Value));
        }
    }

    public static implicit operator char*(PWSTR value) => value.Value;
    public static implicit operator PWSTR(char* value) => new(value);
    public static implicit operator nint(PWSTR value) => (nint)value.Value;

    public static bool operator ==(PWSTR left, PWSTR right) => left.Value == right.Value;
    public static bool operator !=(PWSTR left, PWSTR right) => !(left == right);

    public bool Equals(PWSTR other) => Value == other.Value;
    public override bool Equals(object? obj) => obj is PWSTR other && Equals(other);

    public override int GetHashCode() => ((nuint)Value).GetHashCode();
    public override string? ToString() => Value is null ? null : new string(Value);

    public Span<char> AsSpan() => Value is null ? default : new Span<char>(Value, Length);
}