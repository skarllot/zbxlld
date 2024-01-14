using System.Text.Json;

namespace zbxlld.Windows;

internal static class JsonExtensions
{
    public static void WriteZabbixMacro(
        this Utf8JsonWriter writer,
        string propertyName,
        string? keySuffix,
        string? value)
    {
        var key = string.IsNullOrWhiteSpace(keySuffix)
            ? $"{{#{propertyName}}}"
            : $"{{#{propertyName}{keySuffix}}}";

        writer.WriteString(key, value);
    }
}
