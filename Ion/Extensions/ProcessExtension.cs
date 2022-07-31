using System.Diagnostics;
using Ion.Native;
using Ion.Validation;

namespace Ion.Extensions;

public static class ProcessExtension
{
    public static void Suspend(this Process? process)
    {
        Assert.NotNull(process);

        foreach (ProcessThread thread in process.Threads)
        {
            SuspendResume(thread, handle => Kernel32.SuspendThread(handle));
        }
    }

    public static void Resume(this Process? process)
    {
        Assert.NotNull(process);

        foreach (ProcessThread thread in process.Threads)
        {
            SuspendResume(thread, handle =>
            {
                var suspendCount = 0;

                do
                {
                    suspendCount = Kernel32.ResumeThread(handle);
                }
                while (suspendCount > 0);
            });
        }
    }

    private static void SuspendResume(ProcessThread thread, Action<IntPtr> act)
    {
        var threadHandle = Kernel32.OpenThread(ThreadAccess.SuspendResume, false, thread.Id);

        if (threadHandle == IntPtr.Zero)
        {
            return;
        }

        try
        {
            act(threadHandle);
        }
        finally
        {
            Kernel32.CloseHandle(threadHandle);
        }
    }
}