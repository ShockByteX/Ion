using Ion.Properties;

namespace Ion.Exceptions;

public sealed class FailedToFindFunctionException : Exception
{
    public FailedToFindFunctionException(IProcess process, string modulePath, string functionName, Exception innerException)
        : base(string.Format(Resources.ErrorFailedToFindFunction, functionName, process.Id, process.Name, modulePath), innerException) { }
}