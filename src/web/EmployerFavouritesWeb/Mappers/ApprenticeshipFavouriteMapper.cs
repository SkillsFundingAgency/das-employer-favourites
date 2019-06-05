using System.Linq;
using DfE.EmployerFavourites.Web.Models;

namespace DfE.EmployerFavourites.Web.Mappers
{
    internal class ApprenticeshipFavouriteMapper
    {
        public ApprenticeshipFavouriteViewModel Map(Domain.ReadModel.ApprenticeshipFavourite src)
        {
            return new ApprenticeshipFavouriteViewModel
            {
                Id = src.ApprenticeshipId,
                Title = src.Title,
                IsFramework = src.IsFramework,
                HasTrainingProviders = src.Providers.Any(),
                Ukprn = src.Providers.FirstOrDefault()?.Ukprn
            };
        }
    }
}
