using Ion.Native;
using Ion.Validation;
using System.Text;

namespace Ion;

public static class Drivers
{
    public static nint Find(string driverName)
    {
        var addresses = Enum();

        foreach (var address in addresses)
        {
            var builder = new StringBuilder(256);

            Psapi.GetDeviceDriverBaseName(address, builder, builder.Capacity);

            var name = builder.ToString();

            if (driverName.Equals(name, StringComparison.OrdinalIgnoreCase))
                return address;
        }

        return IntPtr.Zero;
    }

    public static IReadOnlyCollection<nint> Enum()
    {
        Ensure.That(Psapi.EnumDeviceDrivers(null, 0, out var bytesNeeded));

        var addresses = new nint[bytesNeeded / IntPtr.Size];

        Ensure.That(Psapi.EnumDeviceDrivers(addresses, bytesNeeded, out _));

        return addresses;
    }
}