using Ion.Exceptions;
using Ion.Interop;
using Ion.Interop.Handles;
using Ion.Validation;
using System.Runtime.CompilerServices;

namespace Ion.Devices;

public unsafe interface IDevice
{
    nint DeviceHandle { get; }

    void Request<T>(uint code, in T request) where T : unmanaged;

    TResponse Request<TRequest,  TResponse>(uint code, in TRequest request)
        where TRequest : unmanaged
        where TResponse : unmanaged;
}

public sealed unsafe class Device : IDevice, IDisposable
{
    private readonly DeviceHandle _handle;

    private Device(DeviceHandle handle)
    {
        _handle = handle;
    }

    public nint DeviceHandle => _handle;

    public void Request<T>(uint code, in T request) where T : unmanaged
    {
        var size = (uint)sizeof(T);
        ref var reference = ref Unsafe.AsRef(in request);
        var pointer = (nint)Unsafe.AsPointer(ref reference);
        var block = new IoStatusBlock();

        Ensure.NtStatus(Ntdll.NtDeviceIoControlFile(_handle, 0, 0, 0, &block, code, pointer, size, 0, 0));
        //Ensure.That(result, () => $"Failed device request: 0x{code:x} (Address: {((nint)request):x}, Size: {size})");
    }

    public TResponse Request<TRequest,TResponse>(uint code, in TRequest request)
        where TRequest : unmanaged
        where TResponse : unmanaged
    {
        TResponse response = default;
        var requestSize = (uint)sizeof(TRequest);
        var responseSize = (uint)sizeof(TResponse);
        ref var reference = ref Unsafe.AsRef(in request);
        var pointer = (nint)Unsafe.AsPointer(ref reference);
        var block = new IoStatusBlock();

        Ensure.NtStatus(Ntdll.NtDeviceIoControlFile(_handle, 0, 0, 0, &block, code, pointer, requestSize, (nint)(&response), responseSize));

        //Ensure.That(result, () => $"Failed device request: 0x{code:x} (Size: {requestSize})");
        Ensure.That(responseSize == block.Information, () => "Device response output length is not equal to the length of the response type");

        return response;
    }

    public void Dispose()
    {
        _handle.Dispose();
    }

    public static Device Open(string deviceName, FileAccessRights rights = FileAccessRights.FileGenericRead | FileAccessRights.FileGenericWrite,
        FileCreateOptions options = FileCreateOptions.None)
    {
        var handle = Interop.Handles.DeviceHandle.Open(deviceName, rights, options);
        return new Device(handle);
    }

    public static bool Exists(string deviceName)
    {
        try
        {
            using var _ = Interop.Handles.DeviceHandle.Open(deviceName, FileAccessRights.FileGenericRead);
            return true;
        }
        catch (NtStatusException e) when (e.Status is NtStatus.ObjectNameNotFound)
        {
            return false;
        }
    }
}