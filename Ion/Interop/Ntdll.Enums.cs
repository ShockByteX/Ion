using System;
using System.Collections.Generic;
using System.Text;

namespace Ion.Interop;

internal enum FileCreateDisposition : uint
{
    Supersede,
    Open,
    Create,
    OpenIf,
    Overwrite,
    OverwriteIf
}

[Flags]
internal enum ObjectAttributeFlags : uint
{
    None = 0,
    Inherit = 0x00000002,
    Permanent = 0x00000010,
    Exclusive = 0x00000020,
    CaseInsensitive = 0x00000040,
    OpenIf = 0x00000080,
    OpenLink = 0x00000100,
    KernelHandle = 0x00000200,
    ForceAccessCheck = 0x00000400,
    IgnoreImpersonatedDeviceMap = 0x00000800,
    DontReparse = 0x00001000,
    ValidAttributes = 0x00001FF2
}

[Flags]
public enum FileAccessRights : uint
{
    FileReadData = 0x00000001,
    FileListDirectory = 0x00000001,
    FileWriteData = 0x00000002,
    FileAddFile = 0x00000002,
    FileAppendData = 0x00000004,
    FileAddSubdirectory = 0x00000004,
    FileCreatePipeInstance = 0x00000004,
    FileReadEa = 0x00000008,
    FileWriteEa = 0x00000010,
    FileExecute = 0x00000020,
    FileTraverse = 0x00000020,
    FileDeleteChild = 0x00000040,
    FileReadAttributes = 0x00000080,
    FileWriteAttributes = 0x00000100,
    Delete = 0x00010000,
    ReadControl = 0x00020000,
    WriteDAC = 0x00040000,
    WriteOwner = 0x00080000,
    Synchronize = 0x00100000,
    StandardRightsRequired = 0x000F0000,
    StandardRightsRead = 0x00020000,
    StandardRightsWrite = 0x00020000,
    StandardRightsExecute = 0x00020000,
    StandardRightsAll = 0x001F0000,
    SpecificRightsAll = 0x0000FFFF,
    FileAllAccess = 0x001F01FF,
    FileGenericRead = 0x00120089,
    FileGenericWrite = 0x00120116,
    FileGenericExecute = 0x001200A0
}

[Flags]
public enum FileCreateOptions : uint
{
    None = 0,
    DirectoryFile = 0x00000001,
    NonDirectoryFile = 0x00000040,
    WriteThrough = 0x00000002,
    SequentialOnly = 0x00000004,
    RandomAccess = 0x00000800,
    NoIntermediateBuffering = 0x00000008,
    SynchronousIoAlert = 0x00000010,
    SynchronousIoNonAlert = 0x00000020,
    CreateTreeConnection = 0x00000080,
    NoEaKnowledge = 0x00000200,
    OpenReparsePoint = 0x00200000,
    DeleteOnClose = 0x00001000,
    OpenByFileId = 0x00002000,
    OpenForBackupIntent = 0x00004000,
    ReserveOpFilter = 0x00100000,
    OpenRequiringOpLock = 0x00010000,
    CompleteIfOpLocked = 0x00000100,
    OpenForFreeSpaceQuery = 0x00800000,
    ContainsExtendedCreateInformation = 0x10000000,
    NoCompression = 0x00008000,
    DisallowExclusive = 0x00020000,
    SessionAware = 0x00040000,
    OpenNoRecall = 0x00400000,
}