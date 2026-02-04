using BenchmarkDotNet.Running;
using Ion;
using Ion.ConsoleTest;
using Ion.Devices;
using Ion.Interop;
using Ion.Interop.Handles;
using Ion.Memory;
using Ion.Validation;
using System.Runtime.CompilerServices;

BenchmarkRunner.Run<MarshalTests>();

//Ensure.That(DeviceManager.TryFind("Logitech G HUB Virtual Bus Enumerator", out var deviceInfo));

//var deviceNames = DosDevices.ListDeviceNames();

//foreach (var deviceName in deviceNames)
//{
//    var targets = DosDevices.ListDeviceNames(deviceName);

//    foreach (var target in targets)
//    {
//        if (target.Equals(deviceInfo.PhysicalDeviceObjectName))
//            Console.WriteLine($"{deviceName} -> {target}");
//    }
//}

//using var handle = DeviceHandle.Open(@"\??\ROOT#SYSTEM#0001#{dfbedcdd-2148-416d-9e4d-cecc2424128c}");


Console.WriteLine();