using System.Runtime.InteropServices;

namespace Ion.Native;

[StructLayout(LayoutKind.Sequential)]
public struct MemoryBasicInformation
{
    public nint BaseAddress;
    public nint AllocationBase;
    public MemoryProtectionFlags AllocationProtect;
    public long RegionSize;
    public MemoryStateFlags State;
    public MemoryProtectionFlags MemoryProtection;
    public MemoryTypeFlags Type;
}

[StructLayout(LayoutKind.Sequential)]
internal struct MemoryBasicInformation32
{
    public nint BaseAddress;
    public nint AllocationBase;
    public MemoryProtectionFlags AllocationProtect;
    public int RegionSize;
    public MemoryStateFlags State;
    public MemoryProtectionFlags MemoryProtection;
    public MemoryTypeFlags Type;

    public static readonly int StructSize = Marshal.SizeOf(typeof(MemoryBasicInformation32));

    public static implicit operator MemoryBasicInformation(MemoryBasicInformation32 memInfo)
    {
        return new MemoryBasicInformation()
        {
            BaseAddress = memInfo.BaseAddress,
            AllocationBase = memInfo.AllocationBase,
            AllocationProtect = memInfo.AllocationProtect,
            RegionSize = memInfo.RegionSize,
            State = memInfo.State,
            MemoryProtection = memInfo.MemoryProtection,
            Type = memInfo.Type
        };
    }
}

[StructLayout(LayoutKind.Sequential)]
internal struct MemoryBasicInformation64
{
    public nint BaseAddress;
    public nint AllocationBase;
    public MemoryProtectionFlags AllocationProtect;
    public uint Alignment1;
    public long RegionSize;
    public MemoryStateFlags State;
    public MemoryProtectionFlags MemoryProtection;
    public MemoryTypeFlags Type;
    public uint Alignment2;

    public static readonly int StructSize = Marshal.SizeOf(typeof(MemoryBasicInformation64));

    public static implicit operator MemoryBasicInformation(MemoryBasicInformation64 memInfo)
    {
        return new MemoryBasicInformation
        {
            BaseAddress = memInfo.BaseAddress,
            AllocationBase = memInfo.AllocationBase,
            AllocationProtect = memInfo.AllocationProtect,
            RegionSize = memInfo.RegionSize,
            State = memInfo.State,
            MemoryProtection = memInfo.MemoryProtection,
            Type = memInfo.Type
        };
    }
}

internal struct SystemInfo
{
    public ushort ProcessorArchitecture;
    public ushort Reserved;
    public uint PgeSize;
    public nint MinimumApplicationAddress;
    public nint MaximumApplicationAddress;
    public nint ActiveProcessorMask;
    public uint NumberOfProcessors;
    public uint ProcessorType;
    public uint AllocationGranularity;
    public ushort ProcessorLevel;
    public ushort ProcessorRevision;

    public static readonly int StructSize = Marshal.SizeOf(typeof(SystemInfo));
}

[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
internal struct ProcessEntry32
{
    private const int FileNameSize = 260;

    public int StructSize;
    public int Usage;
    public int ProcessId;
    public nint DefaultHeapId;
    public int ModuleId;
    public int Threads;
    public int ParentProcessId;
    public int ClassBase;
    public int Flags;

    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = FileNameSize)]
    public string ProcessName;

    public static ProcessEntry32 Create() => new() { StructSize = Marshal.SizeOf(typeof(ProcessEntry32)) };
}

[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
internal struct ModuleEntry32
{
    public const int ModuleNameSize = 256;
    public const int FileNameSize = 260;

    public int StructSize;
    public int ModuleId;
    public int ProcessId;
    public int GlblcntUsage;
    public int ProccntUsage;
    public nint BaseAddress;
    public int BaseSize;
    public nint Handle;

    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = ModuleNameSize)]
    public string Name;

    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = FileNameSize)]
    public string FileName;

    public static ModuleEntry32 Create() => new() { StructSize = Marshal.SizeOf(typeof(ModuleEntry32)) };
}