using BenchmarkDotNet.Running;
using Ion;
using Ion.ConsoleTest;
using Ion.Devices;
using Ion.Interop;
using Ion.Interop.Handles;
using Ion.Memory;
using Ion.Validation;
using System.Runtime.CompilerServices;

//BenchmarkRunner.Run<MarshalTests>();

using var device = Device.Open(@"\??\ROOT#SYSTEM#0001#{dfbedcdd-2148-416d-9e4d-cecc2424128c}",
    FileAccessRights.FileGenericWrite | FileAccessRights.Synchronize,
    FileCreateOptions.NonDirectoryFile | FileCreateOptions.SynchronousIoNonAlert);

Console.ReadKey(true);
Console.WriteLine("move started");
Move(device, 500, 300);
Console.WriteLine("move finished");
Console.ReadKey(true);

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

static void MoveMouse(IDevice device, sbyte button, sbyte x, sbyte y, sbyte wheel)
{
    var io = new MOUSE_IO
    {
        unk1 = 0,
        button = button,
        x = x,
        y = y,
        wheel = wheel
    };

    device.Request(0x2a2010, in io);

    //if (!callmouse(ref io))
    //{
    //    mouse_close();
    //    mouse_open();
    //}
}

static void Move(IDevice device, int x, int y)
{
    // Чтобы не жечь стек рекурсией, сделаем эквивалентный цикл
    while (Math.Abs(x) > 127 || Math.Abs(y) > 127)
    {
        if (Math.Abs(x) > 127)
        {
            int stepX = Math.Sign(x) * 127;
            MoveMouse(device, 0, (sbyte)stepX, 0, 0);
            x -= stepX;
        }
        else
        {
            MoveMouse(device, 0, (sbyte)x, 0, 0);
            x = 0;
        }

        if (Math.Abs(y) > 127)
        {
            int stepY = Math.Sign(y) * 127;
            MoveMouse(device, 0, 0, (sbyte)stepY, 0);
            y -= stepY;
        }
        else
        {
            MoveMouse(device, 0, 0, (sbyte)y, 0);
            y = 0;
        }
    }

    // Финальный шаг, когда оба влезают в [-127..127]
    MoveMouse(device, 0, (sbyte)x, (sbyte)y, 0);
}

Console.WriteLine();