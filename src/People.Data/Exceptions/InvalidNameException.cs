using System;

namespace People.Data.Exceptions
{
    public class InvalidNameException : Exception
    {
        public InvalidNameException()
        {
        }

        public InvalidNameException(string message)
            : base(message)
        {
        }
    }
}
