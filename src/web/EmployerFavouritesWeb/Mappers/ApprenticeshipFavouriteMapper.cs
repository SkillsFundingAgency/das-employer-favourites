using DfE.EmployerFavourites.Web.Models;
using DfE.EmployerFavourites.Web.Domain;

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
