using Ion.Validation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ion.Interop.Handles;

public class DeviceHandle : IDisposable
{
    private nint _handle;

    private DeviceHandle(nint handle)
    {
        _handle = handle;
    }

    public void Dispose()
    {
        if (_handle != nint.Zero)
        {
            Ntdll.NtClose(_handle);
            _handle = nint.Zero;
        }
    }

    public static unsafe DeviceHandle Open(string deviceName, 
        FileAccessRights rights = FileAccessRights.FileGenericRead | FileAccessRights.FileGenericWrite,
        FileCreateOptions options = FileCreateOptions.None)
    {
        using var name = SafeUnicodeString.Allocate(deviceName);
        var nameStruct = name.Reference;

        var attributes = new ObjectAttributes
        {
            Length = ObjectAttributes.Size,
            RootDirectory = nint.Zero,
            ObjectName = &nameStruct,
            Attributes = ObjectAttributeFlags.None
        };

        var statusBlock = new IoStatusBlock();

        Ensure.ThatSuccess(Ntdll.NtCreateFile(out var handle,
            rights,
            &attributes,
            &statusBlock,
            nint.Zero,
            Ntdll.FileAttributeNormal,
            FileShare.ReadWrite,
            FileCreateDisposition.Open,
            options,
            nint.Zero,
            0));

        Ensure.That(handle != nint.Zero);

        return new DeviceHandle(handle);
    }

    public static implicit operator nint(DeviceHandle handle) => handle._handle;
}