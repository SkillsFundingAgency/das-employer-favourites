using DfE.EmployerFavourites.Domain;
using DfE.EmployerFavourites.Web.Models;

namespace DfE.EmployerFavourites.Web.Mappers
{
    internal class ApprenticeshipFavouriteMapper
    {
        public ApprenticeshipFavouriteViewModel Map(ApprenticeshipFavourite src)
        {
            return new ApprenticeshipFavouriteViewModel
            {
                Id = src.ApprenticeshipId,
                Title = "This is a place holder",
                IsFramework = src.IsFramework
            };
        }
    }
}
