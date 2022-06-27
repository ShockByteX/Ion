using Ion.Properties;

namespace Ion.Exceptions;

internal sealed class FailedToFindProcessException : Exception
{
    public FailedToFindProcessException(object entry) : base(string.Format(Resources.ErrorFailedToFindProcess, entry)) { }
}