using System;
using System.Collections.Generic;
using System.Text;

namespace Ion.Interop.Primitives;

public enum Win32Error
{
    Success = 0,
    InvalidData = 13,
    InsufficientBuffer = 122,
    NoMoreItems = 259,
    NotFound = 1168
}