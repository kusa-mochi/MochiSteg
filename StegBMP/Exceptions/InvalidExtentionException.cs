using System;
using System.Collections.Generic;
using System.Text;

namespace StegBMP
{
    public class InvalidExtentionException : Exception
    {
        public InvalidExtentionException()
        {

        }

        public InvalidExtentionException(string message)
            : base(message)
        {

        }

        public InvalidExtentionException(string message, Exception inner)
            : base(message)
        {

        }
    }
}
