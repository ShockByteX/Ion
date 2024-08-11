using Ion.Properties;

namespace Ion.Exceptions;

internal sealed class FailedToFindProcessException(object entry) 
    : Exception(string.Format(Resources.ErrorFailedToFindProcess, entry))
{ }