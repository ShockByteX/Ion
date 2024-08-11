﻿using System.Runtime.InteropServices;

namespace Ion.PE32;

[StructLayout(LayoutKind.Sequential)]
public struct ImageExportDirectory
{
    public uint Characteristics;
    public uint TimeDateStamp;
    public ushort MajorVersion;
    public ushort MinorVersion;
    public uint Name;
    public uint Base;
    public uint NumberOfFunctions;
    public uint NumberOfNames;
    public int AddressOfFunctions; // RVA from base of image
    public int AddressOfNames; // RVA from base of image
    public int AddressOfNameOrdinals; // RVA from base of image
}