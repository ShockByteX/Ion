using Ion.Properties;

namespace Ion.Exceptions;

public sealed class FailedToLoadModuleException : Exception
{
    public FailedToLoadModuleException(IProcess process, string modulePath, Exception innerException)
        : base(string.Format(Resources.ErrorFailedToLoadModule, modulePath, process.Id, process.Name), innerException) { }
}