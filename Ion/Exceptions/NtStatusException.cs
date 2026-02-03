using Ion.Interop;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ion.Exceptions;

public sealed class NtStatusException(NtStatus status) : EnumStatusException<NtStatus>(status);