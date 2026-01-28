using Ion.Native;
using Ion.Extensions;

namespace Ion.Memory;

public interface IMemoryRegion : IEquatable<IMemoryRegion>, IMemoryPointer
{
    MemoryBasicInformation Information { get; }

    bool IsReadable { get; }
    bool IsWritable { get; }
    bool IsExecutable { get; }
    bool IsGuarded { get; }
    bool IsOperableGuarded { get; }

    bool IsInRange(nint address);
    bool IsInRange(long address);

    IMemoryProtection ChangeProtection(MemoryProtectionFlags protection = MemoryProtectionFlags.ExecuteReadWrite);
    void Release();
}

internal class MemoryRegion : MemoryPointer, IEquatable<MemoryRegion>, IMemoryRegion
{
    public MemoryRegion(IProcessMemory memory, nint address) : base(memory, address) { }

    public MemoryBasicInformation Information
    {
        get
        {
            Memory.Process.Handle.Query(Address, out var memoryInfo);
            return memoryInfo;
        }

    }

    public override bool IsValid => base.IsValid && Information.State != MemoryStateFlags.Free;

    public bool IsReadable => Information.MemoryProtection is MemoryProtectionFlags.ReadOnly
        or MemoryProtectionFlags.ReadWrite
        or MemoryProtectionFlags.ExecuteRead
        or MemoryProtectionFlags.ExecuteReadWrite;

    public bool IsWritable => Information.MemoryProtection is MemoryProtectionFlags.ReadWrite
        or MemoryProtectionFlags.WriteCopy
        or MemoryProtectionFlags.ExecuteReadWrite
        or MemoryProtectionFlags.ExecuteWriteCopy
        or MemoryProtectionFlags.WriteCombine;

    public bool IsExecutable => Information.MemoryProtection is MemoryProtectionFlags.Execute
        or MemoryProtectionFlags.ExecuteRead
        or MemoryProtectionFlags.ExecuteReadWrite
        or MemoryProtectionFlags.ExecuteWriteCopy
        or MemoryProtectionFlags.WriteCombine;

    public bool IsGuarded => Information.MemoryProtection.HasFlag(MemoryProtectionFlags.Guard);
    public bool IsOperableGuarded => (IsReadable | IsWritable | IsExecutable) && IsGuarded;

    public bool IsInRange(nint address) => IsInRange(address.ToInt64());

    public bool IsInRange(long address)
    {
        var regionAddressValue = Information.BaseAddress.ToInt64();
        return address > regionAddressValue && address < regionAddressValue + Information.RegionSize;
    }

    public IMemoryProtection ChangeProtection(MemoryProtectionFlags protection = MemoryProtectionFlags.ExecuteReadWrite)
    {
        return MemoryProtection.Change(Memory.Process.Handle, Address, (int)Information.RegionSize, protection);
    }

    public void Release()
    {
        Memory.Process.Handle.ReleaseMemoryPage(Address);
        Address = nint.Zero;
    }

    public bool Equals(MemoryRegion? other) => Equals((IMemoryRegion?)other);
    public bool Equals(IMemoryRegion? other) => other is not null && Address == other.Address && Information.RegionSize == other.Information.RegionSize;
    public override bool Equals(object? obj) => obj is IMemoryRegion region && Equals(region);
    public override int GetHashCode() => Address.GetHashCode() ^ Information.RegionSize.GetHashCode();

    public override string ToString() => $"Address: 0x{Address.ToInt64():X}, Size: 0x{Information.RegionSize:X}, Protection: {Information.MemoryProtection}";

    public static bool operator ==(MemoryRegion left, MemoryRegion right) => Equals(left, right);
    public static bool operator !=(MemoryRegion left, MemoryRegion right) => !Equals(left, right);
}