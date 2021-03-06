using System;

namespace Extension
{
    public class EnsoException : Exception
    {
        public EnsoException()
        {
        }

        public EnsoException(string message)
            : base(message)
        {
        }

        public EnsoException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
