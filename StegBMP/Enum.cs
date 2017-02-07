using System;
using System.Collections.Generic;
using System.Text;

namespace StegBMP
{
    internal enum FormState : uint
    {
        INITIALIZED,
        FILE_DROP_AVAILABLE,
        READY_TO_WRITE_DATA,
        SHOWING_FILE_LIST,
        MAKING_BMP,
        EXTRACTING_FILES
    }

    internal enum MOCHI_STATUS
    {
        STATUS_SUCCESSFUL = 0,
        STATUS_UNSUCCESSFUL,
        STATUS_INVALID_PARAMETER,
        STATUS_TOO_LARGE_DATA,
        STATUS_NOT_YET
    }
}
