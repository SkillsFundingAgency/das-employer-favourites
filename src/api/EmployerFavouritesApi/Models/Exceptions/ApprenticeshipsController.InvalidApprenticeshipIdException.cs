using System;

namespace DfE.EmployerFavourites.Api.Models.Exceptions
{
    public class InvalidApprenticeshipIdException : Exception
    {
        public InvalidApprenticeshipIdException(string message) : base(message) { }
    }
}
