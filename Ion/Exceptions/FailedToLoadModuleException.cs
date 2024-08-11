using Ion.Properties;

namespace Ion.Exceptions;

public sealed class FailedToLoadModuleException(IProcess process, string modulePath, Exception innerException) 
    : Exception(string.Format(Resources.ErrorFailedToLoadModule, modulePath, process.Id, process.Name), innerException)
{ }