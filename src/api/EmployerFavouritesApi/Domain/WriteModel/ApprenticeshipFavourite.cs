using System.Collections.Generic;
using DfE.EmployerFavourites.Api.Models;

namespace DfE.EmployerFavourites.Api.Domain.WriteModel
{
    public class ApprenticeshipFavourite
    {
        public ApprenticeshipFavourite()
        {
            Ukprns = new List<ProviderData>();
        }

        public string ApprenticeshipId { get; set; }
        public IList<ProviderData> Ukprns { get; set; }

    }
}
