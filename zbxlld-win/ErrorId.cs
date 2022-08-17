namespace zbxlld.Windows
{
    internal static class ErrorId
    {
        // No error detected
        public const int NoError = 0;
        // No command-line parameter was provided
        public const int ParameterNone = 1;
        // An invalid command-line parameter was provided
        public const int ParameterInvalid = 2;
        // The operation to get JSON output resulted in an empty output
        public const int OutputEmpty = 3;
        // The operation to get all filesystem volumes resulted in a out of memory exception
        public const int GetAllVolumesOutOfMemory = 4;
    }
}
