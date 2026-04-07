using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Ion.ConsoleTest;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct MOUSE_IO
{
    public sbyte button;
    public sbyte x;
    public sbyte y;
    public sbyte wheel;
    public sbyte unk1;
}