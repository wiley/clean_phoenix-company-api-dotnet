using System;

namespace CompanyAPI.Domain.Exceptions
{
    [Serializable]
    public class BadRequestException : Exception
    {
        public BadRequestException() { }
    }
}