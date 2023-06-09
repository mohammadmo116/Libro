﻿namespace Libro.Domain.Exceptions.UserExceptions
{
    public class UserExistsException : Exception
    {
        public string _message;
        public string _field;
        public UserExistsException(string field)
          : base($"The {field} is used")
        {
            _field = field;
        }
    }
}
