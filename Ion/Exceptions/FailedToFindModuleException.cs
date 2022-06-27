using Ion.Properties;

namespace Ion.Exceptions;

public sealed class FailedToFindModuleException : Exception
{
    public FailedToFindModuleException(IProcess process, string moduleName) 
        : base(string.Format(Resources.ErrorFailedToFindModule, moduleName, process.Id, process.Name)) { }
}