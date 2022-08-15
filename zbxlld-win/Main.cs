using System;
using System.Collections.Generic;

namespace zbxlld.Windows
{
	internal class MainClass
    {
#if DEBUG
        public const bool DEBUG = true;
        private const string PROGRAM_TITLE_SUFFIX = " DEBUG";
        private static readonly string DEBUG_FILE =
            System.IO.Path.GetDirectoryName(
            System.Reflection.Assembly.GetExecutingAssembly().Location) +
            "\\zbxlld-win-DEBUG.log";
#else
        public const bool DEBUG = false;
        private const string PROGRAM_TITLE_SUFFIX = "";
        private static readonly string DEBUG_FILE = null;
#endif

        public const string PROGRAM_NAME = "zbxlld-win";
        // Latest release: 0.6.1.22
        // Major.Minor.Maintenance.Build
        public const string PROGRAM_VERSION = "0.7.0.30";
        public const string PROGRAM_VERSION_SIMPLE = "0.7";
        public const string PROGRAM_TITLE = PROGRAM_NAME + " " + PROGRAM_VERSION_SIMPLE + PROGRAM_TITLE_SUFFIX;
        private static Logger log = null;

        private static IArgHandler[] ARG_HANDLERS = new IArgHandler[] {
			Discovery.Drive.Default, Discovery.Network.Default, Discovery.Service.Default };

		public static int Main(string[] args)
		{
			string key, keySuffix = null;
            if (DEBUG)
            {
                log = new Logger(MainClass.DEBUG_FILE);
                log.WriteHeader(PROGRAM_NAME, PROGRAM_VERSION);
            }

			if (args.Length < 1) {
				Console.WriteLine("At least one parameter should be provided.");
                if (DEBUG)
                {
                    log.WriteFooter();
                    log.Dispose();
                }
				return (int)ErrorId.ParameterNone;
			} else if (args.Length > 1) {
				if (args [1] != "NULL")
					keySuffix = args [1];
			}

			key = args [0];

			Dictionary<string, IArgHandler> hList =
				new Dictionary<string, IArgHandler>();
			foreach (IArgHandler i in ARG_HANDLERS) {
				foreach (string j in i.GetAllowedArgs()) {
					hList.Add(j, i);
				}
			}

			if (!hList.TryGetValue(args [0], out var val)) {
                if (DEBUG)
                    log.WriteEntry(string.Format("Could not found args[0] = {0}", args[0]));

				Console.WriteLine("Invalid argument");
                Console.WriteLine("Available keys are:");
                foreach (string item in hList.Keys)
                    Console.WriteLine(string.Format(" {0}", item));
				return (int)ErrorId.ParameterInvalid;
			}

			Supplement.JsonOutput jout = val.GetOutput(key);
			if (jout == null) {
				Console.WriteLine("Error getting requested key");
				return (int)ErrorId.OutputEmpty;
			}

			Console.Write(jout.GetOutput(keySuffix));
            if (DEBUG)
            {
                log.WriteFooter();
                log.Dispose();
            }

            return (int)ErrorId.NoError;
		}

        public static void WriteLogEntry(string s)
        {
            log.WriteEntry(s);
        }
	}
}
