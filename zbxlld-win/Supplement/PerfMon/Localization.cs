using Microsoft.Win32;
using System.Collections.Generic;

namespace zbxlld.Windows.Supplement.PerfMon
{
	public class Localization
	{
        private const string ClassFullPath = "zbxlld.Windows.Supplement.PerfMon.Localization";
        private const string KeyPerflibCurlang = @"SOFTWARE\Microsoft\Windows NT\CurrentVersion\Perflib\CurrentLanguage";
        private const string KeyPerflibDefault = @"SOFTWARE\Microsoft\Windows NT\CurrentVersion\Perflib\009";
        private static readonly Dictionary<string, string> counterList = CreateCounterDictionary(KeyPerflibDefault);
        private static readonly Dictionary<string, string> translatedCounterList = CreateCounterDictionary(KeyPerflibCurlang);

        private static Dictionary<string, string> CreateCounterDictionary(string keyName)
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

                if (!result.TryAdd(strCounter[i], strCounter[i + 1]) && MainClass.DEBUG)
                {
                    MainClass.WriteLogEntry(string.Format("{0}..cctor: duplicated key. " +
                                                          "Existing: \"{1}\" \"{2}\". New: \"{3}\" \"{4}\".", ClassFullPath,
                        strCounter[i], result[strCounter[i]], strCounter[i], strCounter[i + 1]));
                }
            }

            if (MainClass.DEBUG)
            {
                MainClass.WriteLogEntry(string.Format("{0}..cctor: ending. " +
                    "counterList.Count: {1}", ClassFullPath, result.Count));
            }

            return result;
        }

		public static string GetName(string id) {
			return counterList[id];
		}

		public static string GetNameForCurrentLanguage(string id) {
			return translatedCounterList[id];
		}
	}
}

