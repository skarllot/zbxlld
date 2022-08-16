using Microsoft.Win32;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;

namespace zbxlld.Windows.Supplement.PerfMon
{
	public sealed class Localization
	{
        private const string KeyPerflibCurlang = @"SOFTWARE\Microsoft\Windows NT\CurrentVersion\Perflib\CurrentLanguage";
        private const string KeyPerflibDefault = @"SOFTWARE\Microsoft\Windows NT\CurrentVersion\Perflib\009";
        private readonly Dictionary<string, string> _counterList;
        private readonly Dictionary<string, string> _translatedCounterList;

        public Localization(ILogger<Localization> logger)
        {
            _counterList = CreateCounterDictionary(KeyPerflibDefault, logger);
            _translatedCounterList = CreateCounterDictionary(KeyPerflibCurlang, logger);
        }

        private static Dictionary<string, string> CreateCounterDictionary(string keyName, ILogger<Localization> logger)
        {
            var regKey = Registry.LocalMachine.OpenSubKey(keyName);
            if (regKey is null) return new Dictionary<string, string>();

            var strCounter = regKey.GetValue("Counter") as string[];
            regKey.Close();
            if (strCounter is null) return new Dictionary<string, string>();

            var result = new Dictionary<string, string>(strCounter.Length / 2);
            for (int i = 0; i < strCounter.Length; i += 2)
            {
                if (string.IsNullOrEmpty(strCounter[i]) ||
                    string.IsNullOrEmpty(strCounter[i + 1]))
                {
                    continue;
                }

                if (!result.TryAdd(strCounter[i], strCounter[i + 1]))
                {
                    logger.LogInformation(
                        "The key {Key} already exists with the value {CurrentValue} but tried to write the value {OtherValue}",
                        strCounter[i],
                        result[strCounter[i]],
                        strCounter[i + 1]);
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
	}
}

