using Ion.Handles;
using Ion.Native;
using Ion.Validation;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace Ion.Threading;

public interface IProcessThread : IEquatable<IProcessThread>
{
    uint Id { get; }
    void Resume();
    void Suspend();
    int Wait();
    int Wait(TimeSpan time);
}

public interface IProcessThreadDisposable : IProcessThread, IDisposable { }

internal sealed class ProcessThread(SafeThreadHandle threadHandle, uint id) : IProcessThreadDisposable
{
    private const int ThreadOperationError = -1;

    public uint Id => id;

    public void Resume() => Ensure.Win32(Kernel32.ResumeThread(threadHandle) is not ThreadOperationError);
    public void Suspend() => Ensure.Win32(Kernel32.SuspendThread(threadHandle) is not ThreadOperationError);
    public int Wait() => Wait(TimeSpan.MaxValue);
    public int Wait(TimeSpan time)
    {
        var milliseconds = time == TimeSpan.MaxValue ? uint.MaxValue : (uint)time.TotalMilliseconds;
        var result = Kernel32.WaitForSingleObject(threadHandle, milliseconds);

        switch (result)
        {
            case WaitObjectResult.Signaled:
            case WaitObjectResult.Abandoned:
                Ensure.Win32(Kernel32.GetExitCodeThread(threadHandle, out var exitCode));
                return exitCode;
            case WaitObjectResult.Timeout:
                throw new TimeoutException($"Waiting for single object timeout exceed: {milliseconds} ms");
            case WaitObjectResult.Failed:
            default:
                var error = Marshal.GetHRForLastWin32Error();
                Marshal.ThrowExceptionForHR(error);
                throw new Win32Exception(error);
        }
    }

    public void Dispose()
    {
        threadHandle.Dispose();
    }

    public bool Equals(IProcessThread? other) => other is not null && Id == other.Id;
    public override bool Equals(object? obj) => obj is IProcessThread other && Equals(other);
    public override int GetHashCode() => Id.GetHashCode();

    public static IProcessThreadDisposable Create(SafeProcessHandle processHandle, IntPtr functionPointer, IntPtr parameterPointer, ThreadCreationFlags flags)
    {
        var threadHandle = Kernel32.CreateRemoteThread(processHandle, IntPtr.Zero, 0, functionPointer, parameterPointer, flags, out var threadId);
        return new ProcessThread(threadHandle, threadId);
    }
}