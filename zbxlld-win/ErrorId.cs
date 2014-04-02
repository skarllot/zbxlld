using System;
using System.Collections.Generic;
using System.Text;

namespace zbxlld.Windows
{
    enum ErrorId : int
    {
        // No error detected
        NoError = 0,
        // No command-line parameter was provided
        ParameterNone = 1,
        // An invalid command-line parameter was provided
        ParameterInvalid = 2,
        // The operation to get JSON output resulted in an empty output
        OutputEmpty = 3,
        // The operation to get all filesystem volumes resulted in a out of memory exception
        GetAllVolumesOutOfMemory = 4
    }
}
