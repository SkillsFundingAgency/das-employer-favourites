using System;

namespace DfE.EmployerFavourites.Api.Models.Exceptions
{
   
        public class InvalidUkprnException : Exception
        {
            public InvalidUkprnException(string message) : base(message) { }
        }

    
}
