using System.Collections.Generic;
using System.Threading.Tasks;
using Refit;

namespace DfE.EmployerFavourites.Web.Infrastructure.FavouritesApiClient
{
    public interface IFavouritesApi
    {
        [Get("/api/apprenticeships/{employerAccountId}")]
        Task<Domain.ReadModel.ApprenticeshipFavourites> GetAsync(string employerAccountId);

        [Put("/api/apprenticeships/{employerAccountId}")]
        Task PutAsync(string employerAccountId, List<Domain.WriteModel.ApprenticeshipFavourite> favourites);
    }
}


