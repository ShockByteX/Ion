using Ion.Exceptions;
using Ion.Interop;
using Ion.Interop.Primitives;
using Ion.Properties;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Ion.Validation;

public sealed class InvariantViolationException : InvalidOperationException
{
    public InvariantViolationException() { }
    public InvariantViolationException(string? message) : base(message) { }
    public InvariantViolationException(string? message, Exception? innerException) : base(message, innerException) { }
}

public static class Ensure
{
    [DoesNotReturn]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Throw<TException>() where TException : Exception, new() => throw new TException();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void That<TException>([DoesNotReturnIf(false)] bool condition) where TException : Exception, new()
    {
        if (!condition)
            Throw<TException>();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void That([DoesNotReturnIf(false)] bool condition, Func<Exception> getException)
    {
        if (!condition)
            throw getException();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void That([DoesNotReturnIf(false)] bool condition, Func<string> getMessage)
    {
        if (!condition)
            throw new InvariantViolationException(getMessage());
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void That([DoesNotReturnIf(false)] bool condition) => That<InvariantViolationException>(condition);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void ThatArgument([DoesNotReturnIf(false)] bool condition, string message, [CallerArgumentExpression(nameof(condition))] string? parameterName = null)
    {
        That(condition, () => new ArgumentException(message, parameterName));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void ThatSuccess(NtStatus status)
    {
        if (status is not NtStatus.Success)
            throw new NtStatusException(status);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T NotNull<T>([NotNull] T? value, [CallerArgumentExpression(nameof(value))] string? parameterName = null)
    {
        return value ?? throw new ArgumentNullException(parameterName);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string NotNullOrEmpty(string? value, [CallerArgumentExpression(nameof(value))] string? parameterName = null)
    {
        return string.IsNullOrEmpty(value)
            ? throw new ArgumentException(string.Format(Resources.ErrorValueCannotBeNullOrEmpty, parameterName), parameterName)
            : value;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void FileExists(string path)
    {
        if (!File.Exists(path))
            throw new FileNotFoundException($"File not found: {path}", path);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void OutOfRange(int value, int min, int max, string parameterName)
    {
        if (value < min || value > max)
            throw new ArgumentOutOfRangeException(parameterName);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void OutOfRange(nint value, nint min, nint max, string parameterName)
    {
        if (value < min || value > max)
            throw new ArgumentOutOfRangeException(parameterName);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Win32([DoesNotReturnIf(false)] bool condition)
    {
        if (!condition)
            Marshal.ThrowExceptionForHR(Marshal.GetHRForLastWin32Error());
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Win32([DoesNotReturnIf(false)] bool condition, Func<string> message) => That(condition, () => new Win32Exception(message()));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Win32(Win32Error error, params Win32Error[] acceptableErrors) => That(error is Win32Error.Success || (Array.IndexOf(acceptableErrors, error) >= 0), () => new Win32Exception((int)error));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void LastWin32(params Win32Error[] acceptableErrors) => Win32((Win32Error)Marshal.GetLastWin32Error(), acceptableErrors);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void IsValid(nint handle) => ThatArgument(handle != nint.Zero, "The handle is invalid", nameof(handle));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void IsValid(SafeHandle handle)
    {
        NotNull(handle, nameof(handle));
        ThatArgument(!handle.IsInvalid, "The handle is not valid", nameof(handle));
        ThatArgument(!handle.IsClosed, "The handle is closed", nameof(handle));
    }
}