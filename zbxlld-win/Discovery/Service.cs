using System.ServiceProcess;
using System.Text.Json;

namespace zbxlld.Windows.Discovery
{
    public class Service
    {
        public void GetAny(Utf8JsonWriter writer, string? keySuffix)
        {
            Get(writer, keySuffix, (ServiceStartMode)(-1));
        }

        public void GetAuto(Utf8JsonWriter writer, string? keySuffix)
        {
            Get(writer, keySuffix, ServiceStartMode.Automatic);
        }

        public void GetManual(Utf8JsonWriter writer, string? keySuffix)
        {
            Get(writer, keySuffix, ServiceStartMode.Manual);
        }

        public void GetDisabled(Utf8JsonWriter writer, string? keySuffix)
        {
            Get(writer, keySuffix, ServiceStartMode.Disabled);
        }

        private void Get(Utf8JsonWriter writer, string? keySuffix, ServiceStartMode filter)
        {
            writer.WriteStartArray();

            foreach (var sc in ServiceController.GetServices())
            {
                if (filter > 0 && sc.StartType != filter)
                    continue;

                writer.WriteStartObject();
                writer.WriteZabbixMacro("SVCNAME", keySuffix, sc.ServiceName);
                writer.WriteZabbixMacro("SVCDESC", keySuffix, sc.DisplayName);
                writer.WriteZabbixMacro("SVCSTATUS", keySuffix, sc.Status.ToString());
                writer.WriteZabbixMacro("SVCSTARTTYPE", keySuffix, sc.StartType.ToString());
                writer.WriteEndObject();
            }

            writer.WriteEndArray();
        }
    }
}