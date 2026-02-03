using System;
using System.Collections.Generic;
using System.Text;

namespace Ion.Devices;

public sealed record DeviceInfo(string Name,
    string Service, 
    string Driver, 
    string PhysicalDeviceObjectName,
    string InstanceId);