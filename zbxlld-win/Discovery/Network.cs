using System.Net.NetworkInformation;
using System.Text.Json;

namespace zbxlld.Windows.Discovery;

public class Network
{
    public void GetAll(Utf8JsonWriter writer, string? keySuffix)
    {
        writer.WriteStartArray();

        var netifs = NetworkInterface.GetAllNetworkInterfaces();
        foreach (var n in netifs)
        {
            if (n.NetworkInterfaceType != NetworkInterfaceType.Loopback &&
                n.NetworkInterfaceType != NetworkInterfaceType.Tunnel)
            {
                writer.WriteStartObject();
                writer.WriteZabbixMacro("IFDESC", keySuffix, n.Description);
                writer.WriteZabbixMacro("IFNAME", keySuffix, n.Name);
                writer.WriteZabbixMacro("IFADDR", keySuffix, n.GetPhysicalAddress().ToString());
                writer.WriteZabbixMacro("IFTYPE", keySuffix, n.NetworkInterfaceType.ToString());
                writer.WriteEndObject();
            }
        }

        writer.WriteEndArray();
    }
}
