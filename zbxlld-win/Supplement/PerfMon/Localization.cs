using Microsoft.Win32;
using Microsoft.Extensions.Logging;

namespace zbxlld.Windows.Supplement.PerfMon;

public sealed partial class Localization
{
    private const string KeyPerflibCurlang = @"SOFTWARE\Microsoft\Windows NT\CurrentVersion\Perflib\CurrentLanguage";
    private const string KeyPerflibDefault = @"SOFTWARE\Microsoft\Windows NT\CurrentVersion\Perflib\009";

    private readonly ILogger<Localization> _logger;
    private readonly Dictionary<string, string> _counterList;
    private readonly Dictionary<string, string> _translatedCounterList;

    public Localization(ILogger<Localization> logger)
    {
        _logger = logger;
        _counterList = CreateCounterDictionary(KeyPerflibDefault);
        _translatedCounterList = CreateCounterDictionary(KeyPerflibCurlang);
    }

    private Dictionary<string, string> CreateCounterDictionary(string keyName)
    {
        var regKey = Registry.LocalMachine.OpenSubKey(keyName);
        if (regKey is null) return new Dictionary<string, string>();

        var strCounter = regKey.GetValue("Counter") as string[];
        regKey.Close();
        if (strCounter is null) return new Dictionary<string, string>();

        var result = new Dictionary<string, string>(strCounter.Length / 2);
        for (var i = 0; i < strCounter.Length; i += 2)
        {
            if (string.IsNullOrEmpty(strCounter[i]) ||
                string.IsNullOrEmpty(strCounter[i + 1]))
            {
                continue;
            }

            if (!result.TryAdd(strCounter[i], strCounter[i + 1]))
            {
                LogDuplicatedKey(strCounter[i], result[strCounter[i]], strCounter[i + 1]);
            }
        }

        return result;
    }

    public string GetName(string id) {
        return _counterList[id];
    }

    public string GetNameForCurrentLanguage(string id) {
        return _translatedCounterList[id];
    }

    [LoggerMessage(
        LogLevel.Information,
        "The key {Key} already exists with the value {CurrentValue} but tried to write the value {OtherValue}")]
    private partial void LogDuplicatedKey(string key, string currentValue, string otherValue);
}