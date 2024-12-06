using System;

namespace CompanyAPI.Domain.Exceptions
{
    [Serializable]
    public class ConflictException : Exception
    {
        public ConflictException(string message = "") : base(message) { }
    }
}