using Ion.Engine;

namespace Ion.Detours;

public sealed class Detour : IDisposable
{
    private const int JumpInstructionLength = 5;
    private const byte JumpInstructionByte = 0xE9;
    private const byte NopInstructionByte = 0x90;

    private readonly IProcess _process;
    private readonly byte[] _originalData;
    private readonly byte[] _injectedData;
    private readonly nint _caveAddress;
    private readonly int _caveLength;

    private bool _isActive;

    public Detour(IProcess process, nint address, byte[] originalData, byte[] injectedData, nint caveAddress, int caveLength)
    {
        _process = process;
        _originalData = originalData;
        _injectedData = injectedData;
        _caveAddress = caveAddress;
        _caveLength = caveLength;

        Address = address;
        AdditionalAddress = caveAddress + caveLength;
    }

    public bool IsActive { get => _isActive; set => SetState(value); }
    public nint Address { get; private set; }
    public nint AdditionalAddress { get; }

    private void SetState(bool state)
    {
        if (_isActive == state) return;

        _process[Address].Write(0, _isActive ? _originalData : _injectedData);

        _isActive = state;
    }

    public void Dispose()
    {
        IsActive = false;
        Address = nint.Zero;
        _process[_caveAddress].Write(0, Enumerable.Range(0, _caveLength).Select(_ => (byte)0).ToArray());
    }

    public static Detour Create(IProcess process, nint address, nint caveAddress, byte[] caveData, int sourceLength, int entryPointOffset = 0)
    {
        var caveLength = caveData.Length;
        var originalData = process[address].Read(0, sourceLength);

        process[caveAddress].Write(0, caveData);

        var jumpData = EvaluateJumpBytes(address + JumpInstructionLength, caveAddress + entryPointOffset);
        var injectedData = jumpData.Concat(Enumerable.Range(0, sourceLength - JumpInstructionLength).Select(_ => NopInstructionByte)).ToArray();

        return new Detour(process, address, originalData, injectedData, caveAddress, caveLength);
    }

    public static nint FindCodeCave(IProcess process, nint address, int size)
    {
        var module = process.Modules[0];
        var data = process[address].Read(0, (int)(module.BaseAddress.ToInt64() + module.Size - address.ToInt64()));
        var signature = Enumerable.Range(0, size << 1).Select(_ => (byte)0).ToArray();
        var offset = SignatureScanner.Scan(data, signature, 0x01)[0] + size;

        return address + offset;
    }

    private static IEnumerable<byte> EvaluateJumpBytes(nint sourceAddress, nint targetAddress)
    {
        var operand = (int)(targetAddress.ToInt64() - sourceAddress.ToInt64());
        var operandData = BitConverter.GetBytes(operand);

        return new[] { JumpInstructionByte }.Concat(operandData).ToArray();
    }
}