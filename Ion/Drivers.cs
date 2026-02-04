using Ion.Interop;
using Ion.Validation;
using System.Text;

namespace Ion;

public static class Drivers
{
    public static nint Find(string driverName)
    {
        var addresses = List();

        foreach (var address in addresses)
        {
            var builder = new StringBuilder(256);

            Psapi.GetDeviceDriverBaseName(address, builder, builder.Capacity);

            var name = builder.ToString();

            if (driverName.Equals(name, StringComparison.OrdinalIgnoreCase))
                return address;
        }

        return nint.Zero;
    }

    public static IReadOnlyCollection<nint> List()
    {
        Ensure.That(Psapi.EnumDeviceDrivers(null, 0, out var bytesNeeded));

        var addresses = new nint[bytesNeeded / nint.Size];

        Ensure.That(Psapi.EnumDeviceDrivers(addresses, bytesNeeded, out _));

        return addresses;
    }
}