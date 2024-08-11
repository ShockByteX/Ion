using Ion.Properties;

namespace Ion.Exceptions;

public sealed class FailedToFindFunctionException(IProcess process, string modulePath, string functionName, Exception innerException)
    : Exception(string.Format(Resources.ErrorFailedToFindFunction, functionName, process.Id, process.Name, modulePath), innerException)
{ }