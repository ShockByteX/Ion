using Ion.Properties;

namespace Ion.Exceptions;

public sealed class FailedToFindModuleException(IProcess process, string moduleName)
    : Exception(string.Format(Resources.ErrorFailedToFindModule, moduleName, process.Id, process.Name))
{ }