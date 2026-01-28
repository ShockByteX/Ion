using System.Runtime.InteropServices;
using Ion.Handles;

namespace Ion.Native;

internal unsafe partial class Kernel32
{
    public const string LibraryName = "kernel32.dll";

    [DllImport(LibraryName, CharSet = CharSet.Auto, SetLastError = true)]
    public static extern nint GetModuleHandle(string lpModuleName);

    [DllImport(LibraryName, ExactSpelling = true, SetLastError = true)]
    public static extern SafeProcessHandle OpenProcess(ProcessAccessFlags dwDesiredAccess, [MarshalAs(UnmanagedType.Bool)] bool bInheritHandle, int dwProcessId);

    [DllImport(LibraryName, ExactSpelling = true, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool CloseHandle(nint hObject);

    [DllImport(LibraryName, ExactSpelling = true, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool DuplicateHandle(nint hSourceProcessHandle, nint hSourceHandle, nint hTargetProcessHandle, out nint lpTargetHandle, uint dwDesiredAccess, [MarshalAs(UnmanagedType.Bool)] bool bInheritHandle, uint dwOptions);

    [DllImport(LibraryName, SetLastError = true, CharSet = CharSet.Ansi)]
    public static extern nint GetProcAddress(nint hModule, string procName);

    [DllImport(LibraryName, ExactSpelling = true, SetLastError = true)]
    public static extern bool VirtualProtectEx(SafeProcessHandle hProcess, nint lpAddress, int dwSize, MemoryProtectionFlags flNewProtect, out MemoryProtectionFlags lpflOldProtect);

    [DllImport(LibraryName, EntryPoint = "VirtualQueryEx", SetLastError = true)]
    public static extern int VirtualQueryEx32(SafeProcessHandle hProcess, nint lpAddress, out MemoryBasicInformation32 lpBuffer, int dwLength);

    [DllImport(LibraryName, EntryPoint = "VirtualQueryEx", SetLastError = true)]
    public static extern int VirtualQueryEx64(SafeProcessHandle hProcess, nint lpAddress, out MemoryBasicInformation64 lpBuffer, int dwLength);

    [DllImport(LibraryName, SetLastError = true)]
    public static extern nint VirtualAllocEx(SafeProcessHandle hProcess, nint lpAddress, int dwSize, MemoryAllocationFlags flAllocationType, MemoryProtectionFlags flProtect);

    [DllImport(LibraryName, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool VirtualFreeEx(SafeProcessHandle hProcess, nint lpAddress, int dwSize, MemoryReleaseFlags dwFreeType);

    [DllImport(LibraryName, EntryPoint = "RtlMoveMemory", SetLastError = true)]
    public static extern void MoveMemory(void* dest, void* src, int size);

    [DllImport(LibraryName, ExactSpelling = true, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool ReadProcessMemory(SafeProcessHandle hProcess, nint lpBaseAddress, [Out] byte[] lpBuffer, int dwSize, out int lpNumberOfBytesRead);

    [DllImport(LibraryName, ExactSpelling = true, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool WriteProcessMemory(SafeProcessHandle hProcess, nint lpBaseAddress, byte[] lpBuffer, int nSize, out int lpNumberOfBytesWritten);

    [DllImport(LibraryName, SetLastError = true)]
    public static extern void GetSystemInfo(out SystemInfo lpSystemInfo);

    [DllImport(LibraryName, CharSet = CharSet.Auto, SetLastError = true)]
    public static extern SafeSnapshotHandle CreateToolhelp32Snapshot(SnapshotFlags flags, int processId);

    [DllImport(LibraryName, CharSet = CharSet.Auto, SetLastError = true)]
    public static extern bool Process32First(SafeSnapshotHandle handle, ref ProcessEntry32 entry);

    [DllImport(LibraryName, CharSet = CharSet.Auto, SetLastError = true)]
    public static extern bool Process32Next(SafeSnapshotHandle handle, ref ProcessEntry32 entry);

    [DllImport(LibraryName, CharSet = CharSet.Auto, SetLastError = true)]
    public static extern bool Module32First(SafeSnapshotHandle handle, ref ModuleEntry32 entry);

    [DllImport(LibraryName, CharSet = CharSet.Auto, SetLastError = true)]
    public static extern bool Module32Next(SafeSnapshotHandle handle, ref ModuleEntry32 entry);

    [DllImport(LibraryName, SetLastError = true, CharSet = CharSet.Unicode)]
    public static extern nint LoadLibrary(string lpFileName);

    [DllImport(LibraryName, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool FreeLibrary(nint hModule);

    [DllImport(LibraryName)]
    public static extern nint OpenThread(ThreadAccess dwDesiredAccess, bool bInheritHandle, int dwThreadId);

    [DllImport(LibraryName)]
    public static extern uint SuspendThread(nint hThread);

    [DllImport(LibraryName)]
    public static extern int ResumeThread(nint hThread);

    public static int VirtualQueryEx(SafeProcessHandle hProcess, nint lpAddress, out MemoryBasicInformation lpBuffer)
    {
        int result;

        switch (nint.Size)
        {
            case 4:
                result = VirtualQueryEx32(hProcess, lpAddress, out var memoryInfo32, MemoryBasicInformation32.StructSize);
                lpBuffer = memoryInfo32;
                break;
            case 8:
                result = VirtualQueryEx64(hProcess, lpAddress, out var memoryInfo64, MemoryBasicInformation64.StructSize);
                lpBuffer = memoryInfo64;
                break;
            default: throw new NotSupportedException();
        }

        return result;
    }
}