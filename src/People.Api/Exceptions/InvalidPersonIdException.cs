using System;

namespace People.Api.Exceptions
{
    public class InvalidPersonIdException : Exception
    {
        public InvalidPersonIdException()
        {
        }

        public InvalidPersonIdException(string message)
            : base(message)
        {
        }
    }
}
