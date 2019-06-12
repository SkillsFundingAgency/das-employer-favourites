using System;

namespace DfE.EmployerFavourites.Web.Application.Exceptions
{
    [Serializable]
    public class EntityNotFoundException : Exception
    {
        public EntityNotFoundException(string message) : base(message) { }
    }
}
