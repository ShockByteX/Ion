using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Ion.Interop;

[StructLayout(LayoutKind.Sequential)]
internal unsafe struct SP_DEVINFO_DATA
{
    public uint cbSize;
    public Guid ClassGuid;
    public uint DevInst;
    public nint Reserved;
}