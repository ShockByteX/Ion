using System;
using System.Collections.Generic;
using System.Text;

namespace Ion.Interop;

[Flags]
internal enum SetupDiGetClassDevsFlags : uint
{
    Default = 0x1,
    Present = 0x2,
    AllClasses = 0x4,
    Profile = 0x8,
    DeviceInterface = 0x10
}

internal enum SetupDiRegistryProperty : uint
{
    DeviceDescription = 0,
    HardwareId = 1,
    CompatibleIds = 2,
    Unused0 = 3,
    Service = 4,
    Unused1 = 5,
    Unused2 = 6,
    Class = 7,
    ClassGuid = 8,
    Driver = 9,
    ConfigFlags = 10,
    Mfg = 11,
    FriendlyName = 12,
    LocationInformation = 13,
    PhysicalDeviceObjectName = 14,
    Capabilities = 15,
    UINumber = 16,
    UpperFilters = 17,
    LowerFilters = 18,
    BusTypeGuid = 19,
    LegacyBusType = 20,
    BusNumber = 21,
    EnumeratorName = 22,
    Security = 23,
    SecuritySDS = 24,
    DeviceType = 25,
    Exclusive = 26,
    Characteristics = 27,
    Address = 28,
    UINumberDescFormat = 29,
    DevicePowerData = 30,
    RemovalPolicy = 31,
    RemovalPolicyHWDefault = 32,
    RemovalPolicyOverride = 33,
    InstallState = 34,
    LocationPaths = 35,
    BaseContainerId = 36,
    MaximumProperty = 37
}