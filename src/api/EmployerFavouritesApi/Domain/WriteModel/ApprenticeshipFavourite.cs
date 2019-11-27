using System.Collections.Generic;
using DfE.EmployerFavourites.Api.Models;

namespace DfE.EmployerFavourites.Api.Domain.WriteModel
{
    public class ApprenticeshipFavourite
    {
        public ApprenticeshipFavourite()
        {
            Providers = new List<Provider>();
        }

        public string ApprenticeshipId { get; set; }
        public IList<Provider> Providers { get; set; }

       
    }
}
