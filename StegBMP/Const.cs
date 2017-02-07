using System;
using System.Collections.Generic;
using System.Text;

namespace StegBMP
{
    internal class Const
    {
        internal const long UNDEFINED_MAX_DATA_SIZE = 0L;
        internal byte[] BITMASK = { 0xFF, 0xFE, 0xFC, 0xF8, 0xF0, 0xE0, 0xC0, 0x80, 0x00 };
        internal const string STRING_PROGRAM_IDENTIFIER = "M-StegBMP";
        internal const string STRING_INVALID_PROGRAM_IDENTIFIER = "Invalid program identifier";
    }
}
